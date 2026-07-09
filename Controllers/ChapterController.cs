using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;

namespace PRN221_FinalProject_Group3.Controllers;

public class ChapterController : Controller
{
    private readonly IChapterService _chapterService;

    public ChapterController(IChapterService chapterService)
    {
        _chapterService = chapterService;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(
        int id,
        CancellationToken cancellationToken)
    {
        var viewModel = await _chapterService.GetChapterDetailAsync(id, cancellationToken);

        if (viewModel is null)
        {
            return NotFound();
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Comment(
        ChapterCommentInputViewModel model,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return RedirectToAction(
                "Login",
                "Account",
                new { returnUrl = Url.Action(nameof(Detail), "Chapter", new { id = model.ChapterId }) });
        }

        if (!ModelState.IsValid)
        {
            TempData["CommentError"] = "Vui lòng nhập nội dung bình luận.";
            return RedirectToAction(nameof(Detail), new { id = model.ChapterId });
        }

        var result = await _chapterService.AddCommentAsync(
            model,
            userId,
            cancellationToken);

        if (!result.Succeeded)
        {
            TempData["CommentError"] = result.Error ?? "Không thể gửi bình luận.";
        }

        return RedirectToAction(nameof(Detail), new { id = model.ChapterId });
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out userId);
    }
}
