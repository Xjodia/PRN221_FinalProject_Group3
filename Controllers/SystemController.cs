using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;

namespace PRN221_FinalProject_Group3.Controllers;

[Authorize(Roles = "Admin")]
public class SystemController : Controller
{
    private readonly ISystemService _systemService;

    public SystemController(ISystemService systemService)
    {
        _systemService = systemService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = await _systemService.GetHomeAsync(cancellationToken);
        return View(model);
    }

    public async Task<IActionResult> Novels(
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var model = await _systemService.GetNovelsAsync(
            showDeleted: false,
            searchTerm,
            cancellationToken);

        return View(model);
    }

    public async Task<IActionResult> DeletedNovels(
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var model = await _systemService.GetNovelsAsync(
            showDeleted: true,
            searchTerm,
            cancellationToken);

        return View(nameof(Novels), model);
    }

    public async Task<IActionResult> Users(
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var model = await _systemService.GetUsersAsync(
            showDeleted: false,
            searchTerm,
            cancellationToken);

        return View(model);
    }

    public async Task<IActionResult> DeletedUsers(
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var model = await _systemService.GetUsersAsync(
            showDeleted: true,
            searchTerm,
            cancellationToken);

        return View(nameof(Users), model);
    }

    public async Task<IActionResult> UserDetail(
        int id,
        CancellationToken cancellationToken)
    {
        var model = await _systemService.GetUserDetailAsync(id, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser(
        SystemUserEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return await ReturnUserDetailWithErrorsAsync(
                model.Id,
                "Thông tin tài khoản chưa hợp lệ.",
                cancellationToken);
        }

        var result = await _systemService.UpdateUserAsync(
            model,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            return await ReturnUserDetailWithErrorsAsync(
                model.Id,
                result.Errors.Values.FirstOrDefault() ?? "Không thể cập nhật tài khoản.",
                cancellationToken);
        }

        TempData["SuccessMessage"] = "Đã cập nhật tài khoản.";
        return RedirectToAction(nameof(UserDetail), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetUserPassword(
        SystemPasswordResetViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return await ReturnUserDetailWithErrorsAsync(
                model.UserId,
                "Mật khẩu mới chưa hợp lệ.",
                cancellationToken);
        }

        var result = await _systemService.ResetUserPasswordAsync(
            model,
            cancellationToken);

        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded
                ? "Đã cấp lại mật khẩu mới cho tài khoản."
                : result.Errors.Values.FirstOrDefault() ?? "Không thể cấp lại mật khẩu.";

        return RedirectToAction(nameof(UserDetail), new { id = model.UserId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteNovel(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _systemService.SoftDeleteNovelAsync(id, cancellationToken);

        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded
                ? "Đã chuyển truyện vào danh sách đã xóa."
                : result.Errors.Values.FirstOrDefault() ?? "Không thể xóa truyện.";

        return RedirectToAction(nameof(Novels));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreNovel(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _systemService.RestoreNovelAsync(id, cancellationToken);

        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded
                ? "Đã khôi phục truyện."
                : result.Errors.Values.FirstOrDefault() ?? "Không thể khôi phục truyện.";

        return RedirectToAction(nameof(DeletedNovels));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _systemService.SoftDeleteUserAsync(
            id,
            GetCurrentUserId(),
            cancellationToken);

        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded
                ? "Đã chuyển tài khoản vào danh sách đã xóa."
                : result.Errors.Values.FirstOrDefault() ?? "Không thể xóa tài khoản.";

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreUser(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _systemService.RestoreUserAsync(id, cancellationToken);

        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded
                ? "Đã khôi phục tài khoản."
                : result.Errors.Values.FirstOrDefault() ?? "Không thể khôi phục tài khoản.";

        return RedirectToAction(nameof(DeletedUsers));
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : 0;
    }

    private void AddErrors(Dictionary<string, string> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }
    }

    private async Task<IActionResult> ReturnUserDetailWithErrorsAsync(
        int userId,
        string errorMessage,
        CancellationToken cancellationToken)
    {
        TempData["ErrorMessage"] = errorMessage;

        var viewModel = await _systemService.GetUserDetailAsync(userId, cancellationToken);
        if (viewModel is null)
        {
            return NotFound();
        }

        return View(nameof(UserDetail), viewModel);
    }
}
