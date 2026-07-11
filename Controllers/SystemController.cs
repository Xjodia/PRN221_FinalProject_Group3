using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
}
