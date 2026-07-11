using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;

namespace PRN221_FinalProject_Group3.Controllers;

public class NovelController : Controller
{
    private readonly INovelService _novelService;

    public NovelController(INovelService novelService)
    {
        _novelService = novelService;
    }

    public async Task<IActionResult> Detail(
        int id,
        CancellationToken cancellationToken)
    {
        var viewModel = await _novelService.GetNovelDetailAsync(id, cancellationToken);

        if (viewModel is null)
        {
            return NotFound();
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Search(
        string? q,
        string? tab,
        string? status,
        string? sort,
        CancellationToken cancellationToken)
    {
        var viewModel = await _novelService.SearchAsync(
            q,
            tab,
            status,
            sort,
            cancellationToken);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Comment(
        NovelCommentInputViewModel model,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return RedirectToAction(
                "Login",
                "Account",
                new { returnUrl = Url.Action(nameof(Detail), "Novel", new { id = model.NovelId }) });
        }

        if (!ModelState.IsValid)
        {
            TempData["CommentError"] = "Vui lòng nhập nội dung bình luận.";
            return RedirectToAction(nameof(Detail), new { id = model.NovelId });
        }

        var result = await _novelService.AddCommentAsync(
            model,
            userId,
            cancellationToken);

        if (!result.Succeeded)
        {
            TempData["CommentError"] = result.Error ?? "Không thể gửi bình luận.";
        }

        return RedirectToAction(nameof(Detail), new { id = model.NovelId });
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out userId);
    }
}
