using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN221_FinalProject_Group3.Models;
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

        var result = await _userService.StartRegistrationAsync(
            model,
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            return View(model);
        }

        return RedirectToAction(
            nameof(VerifyRegistration),
            new { flowId = result.FlowId });
    }

    [HttpGet]
    public async Task<IActionResult> VerifyRegistration(
        Guid flowId,
        CancellationToken cancellationToken)
    {
        var model = await BuildVerificationViewModelAsync(
            flowId,
            EmailVerificationPurpose.Registration,
            cancellationToken);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyRegistration(
        VerifyEmailCodeViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateVerificationViewModelAsync(
                model,
                EmailVerificationPurpose.Registration,
                cancellationToken);
            return View(model);
        }

        var result = await _userService.CompleteRegistrationAsync(
            model.FlowId,
            model.Code,
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            await PopulateVerificationViewModelAsync(
                model,
                EmailVerificationPurpose.Registration,
                cancellationToken,
                addErrors: false);
            return View(model);
        }

        TempData["SuccessMessage"] =
            "Đã xác thực email và tạo tài khoản thành công. Hãy đăng nhập.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.StartPasswordResetAsync(
            model,
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            return View(model);
        }

        return RedirectToAction(
            nameof(VerifyPasswordReset),
            new { flowId = result.FlowId });
    }

    [HttpGet]
    public async Task<IActionResult> VerifyPasswordReset(
        Guid flowId,
        CancellationToken cancellationToken)
    {
        var model = await BuildVerificationViewModelAsync(
            flowId,
            EmailVerificationPurpose.PasswordReset,
            cancellationToken);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyPasswordReset(
        VerifyEmailCodeViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateVerificationViewModelAsync(
                model,
                EmailVerificationPurpose.PasswordReset,
                cancellationToken);
            return View(model);
        }

        var result = await _userService.VerifyPasswordResetCodeAsync(
            model.FlowId,
            model.Code,
            cancellationToken);

        if (!result.Succeeded || string.IsNullOrWhiteSpace(result.ResetToken))
        {
            AddErrors(result.Errors);
            await PopulateVerificationViewModelAsync(
                model,
                EmailVerificationPurpose.PasswordReset,
                cancellationToken,
                addErrors: false);
            return View(model);
        }

        return RedirectToAction(
            nameof(ResetPassword),
            new
            {
                flowId = result.FlowId,
                token = result.ResetToken
            });
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(
        Guid flowId,
        string token,
        CancellationToken cancellationToken)
    {
        if (!await _userService.IsResetTokenValidAsync(
                flowId,
                token,
                cancellationToken))
        {
            TempData["ErrorMessage"] =
                "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        return View(new ResetPasswordViewModel
        {
            FlowId = flowId,
            Token = token
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordViewModel model,
        CancellationToken cancellationToken)
    {
        if (!await _userService.IsResetTokenValidAsync(
                model.FlowId,
                model.Token,
                cancellationToken))
        {
            TempData["ErrorMessage"] =
                "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.ResetPasswordAsync(
            model,
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            return View(model);
        }

        TempData["SuccessMessage"] =
            "Đã đặt lại mật khẩu thành công. Hãy đăng nhập bằng mật khẩu mới.";
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
            new(ClaimTypes.NameIdentifier, GetCurrentUserId().ToString()),
            new(ClaimTypes.Name, displayName),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            properties);
    }

    private async Task<VerifyEmailCodeViewModel> BuildVerificationViewModelAsync(
        Guid flowId,
        EmailVerificationPurpose purpose,
        CancellationToken cancellationToken)
    {
        var model = new VerifyEmailCodeViewModel { FlowId = flowId };
        await PopulateVerificationViewModelAsync(
            model,
            purpose,
            cancellationToken);
        return model;
    }

    private async Task PopulateVerificationViewModelAsync(
        VerifyEmailCodeViewModel model,
        EmailVerificationPurpose purpose,
        CancellationToken cancellationToken,
        bool addErrors = true)
    {
        var result = await _userService.GetVerificationAsync(
            model.FlowId,
            purpose,
            cancellationToken);

        if (result.Succeeded)
        {
            model.Email = result.Email ?? string.Empty;
            model.ExpiresAt = result.ExpiresAt ?? DateTimeOffset.UtcNow;
            return;
        }

        model.ExpiresAt = DateTimeOffset.UtcNow;
        if (addErrors)
        {
            AddErrors(result.Errors);
        }
    }
}
