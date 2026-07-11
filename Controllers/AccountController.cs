using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;

namespace PRN221_FinalProject_Group3.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        RegisterViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.RegisterUserAsync(
            model,
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return View(model);
        }

        TempData["SuccessMessage"] =
            "Đã tạo tài khoản thành công, xin hãy đăng nhập.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.LoginAsync(model, cancellationToken);

        if (!result.Succeeded || result.User is null)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Error ?? "Không thể đăng nhập.");
            return View(model);
        }

        var user = result.User;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            AllowRefresh = true
        };

        if (model.RememberMe)
        {
            properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14);
        }

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            properties);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl)
            && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Settings(CancellationToken cancellationToken)
    {
        var model = await _userService.GetAccountSettingsAsync(
            GetCurrentUserId(),
            cancellationToken);

        if (model is null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(
        ProfileEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var settingsModel = await BuildSettingsViewModelAsync(cancellationToken);
            settingsModel.Profile = model;
            return View(nameof(Settings), settingsModel);
        }

        var result = await _userService.UpdateProfileAsync(
            model,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            var settingsModel = await BuildSettingsViewModelAsync(cancellationToken);
            settingsModel.Profile = model;
            return View(nameof(Settings), settingsModel);
        }

        await RefreshSignInAsync(model.DisplayName, model.Email);
        TempData["SuccessMessage"] = "Đã cập nhật hồ sơ cá nhân.";
        return RedirectToAction(nameof(Settings));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var settingsModel = await BuildSettingsViewModelAsync(cancellationToken);
            settingsModel.Password = model;
            return View(nameof(Settings), settingsModel);
        }

        var result = await _userService.ChangePasswordAsync(
            model,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            var settingsModel = await BuildSettingsViewModelAsync(cancellationToken);
            settingsModel.Password = model;
            return View(nameof(Settings), settingsModel);
        }

        TempData["SuccessMessage"] = "Đã đổi mật khẩu thành công.";
        return RedirectToAction(nameof(Settings));
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : 0;
    }

    private async Task<AccountSettingsViewModel> BuildSettingsViewModelAsync(
        CancellationToken cancellationToken)
    {
        return await _userService.GetAccountSettingsAsync(
            GetCurrentUserId(),
            cancellationToken)
            ?? new AccountSettingsViewModel();
    }

    private void AddErrors(Dictionary<string, string> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }
    }

    private async Task RefreshSignInAsync(string displayName, string email)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = authenticateResult.Properties ?? new AuthenticationProperties();
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "Reader";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, GetCurrentUserId().ToString()),
            new Claim(ClaimTypes.Name, displayName),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            properties);
    }
}
