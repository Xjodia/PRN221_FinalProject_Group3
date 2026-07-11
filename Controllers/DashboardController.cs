using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;

namespace PRN221_FinalProject_Group3.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var model = await _dashboardService.GetHomeAsync(userId, cancellationToken);
        return View(model);
    }

    public async Task<IActionResult> Novels(string? searchTerm, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var model = await _dashboardService.GetMyNovelsAsync(userId, searchTerm, cancellationToken);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> CreateNovel(CancellationToken cancellationToken)
    {
        var model = await _dashboardService.GetCreateNovelFormAsync(cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNovel(
        NovelFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await ReloadNovelFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _dashboardService.CreateNovelAsync(
            model,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            await ReloadNovelFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã tạo truyện thành công.";
        return RedirectToAction(nameof(Detail), new { id = result.EntityId });
    }

    [HttpGet]
    public async Task<IActionResult> EditNovel(int id, CancellationToken cancellationToken)
    {
        var model = await _dashboardService.GetEditNovelFormAsync(id, GetCurrentUserId(), cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditNovel(
        NovelFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await ReloadNovelFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var result = await _dashboardService.UpdateNovelAsync(
            model,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            await ReloadNovelFormOptionsAsync(model, cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã cập nhật truyện.";
        return RedirectToAction(nameof(Detail), new { id = result.EntityId });
    }

    public async Task<IActionResult> Detail(int id, CancellationToken cancellationToken)
    {
        var model = await _dashboardService.GetNovelDetailAsync(id, GetCurrentUserId(), cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteNovel(int id, CancellationToken cancellationToken)
    {
        var result = await _dashboardService.DeleteNovelAsync(id, GetCurrentUserId(), cancellationToken);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Errors.Values.FirstOrDefault() ?? "Không thể xóa truyện.";
            return RedirectToAction(nameof(Novels));
        }

        TempData["SuccessMessage"] = "Đã xóa truyện.";
        return RedirectToAction(nameof(Novels));
    }

    [HttpGet]
    public async Task<IActionResult> CreateChapter(int novelId, CancellationToken cancellationToken)
    {
        var model = await _dashboardService.GetCreateChapterFormAsync(
            novelId,
            GetCurrentUserId(),
            cancellationToken);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateChapter(
        ChapterFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _dashboardService.CreateChapterAsync(
            model,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã thêm chương mới.";
        return RedirectToAction(nameof(Detail), new { id = model.NovelId });
    }

    [HttpGet]
    public async Task<IActionResult> EditChapter(int id, CancellationToken cancellationToken)
    {
        var model = await _dashboardService.GetEditChapterFormAsync(
            id,
            GetCurrentUserId(),
            cancellationToken);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditChapter(
        ChapterFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _dashboardService.UpdateChapterAsync(
            model,
            GetCurrentUserId(),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã cập nhật chương.";
        return RedirectToAction(nameof(Detail), new { id = model.NovelId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteChapter(int id, CancellationToken cancellationToken)
    {
        var result = await _dashboardService.DeleteChapterAsync(id, GetCurrentUserId(), cancellationToken);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Errors.Values.FirstOrDefault() ?? "Không thể xóa chương.";
            return RedirectToAction(nameof(Novels));
        }

        TempData["SuccessMessage"] = "Đã xóa chương.";
        return RedirectToAction(nameof(Detail), new { id = result.EntityId });
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : 0;
    }

    private async Task ReloadNovelFormOptionsAsync(
        NovelFormViewModel model,
        CancellationToken cancellationToken)
    {
        var freshModel = model.Id.HasValue
            ? await _dashboardService.GetEditNovelFormAsync(model.Id.Value, GetCurrentUserId(), cancellationToken)
            : await _dashboardService.GetCreateNovelFormAsync(cancellationToken);

        model.CategoryOptions = freshModel?.CategoryOptions ?? [];
    }

    private void AddErrors(Dictionary<string, string> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }
    }
}
