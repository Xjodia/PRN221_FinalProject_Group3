using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN221_FinalProject_Group3.Services.Interface;

namespace PRN221_FinalProject_Group3.Controllers;

[Authorize]
public class LibraryController : Controller
{
    private readonly ILibraryService _libraryService;

    public LibraryController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    public async Task<IActionResult> Following(
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var model = await _libraryService.GetFollowedNovelsAsync(
            GetCurrentUserId(),
            searchTerm,
            cancellationToken);

        return View(model);
    }

    public async Task<IActionResult> History(CancellationToken cancellationToken)
    {
        var model = await _libraryService.GetReadingHistoryAsync(
            GetCurrentUserId(),
            cancellationToken);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFollow(
        int novelId,
        CancellationToken cancellationToken)
    {
        var result = await _libraryService.RemoveFollowAsync(
            GetCurrentUserId(),
            novelId,
            cancellationToken);

        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded
                ? "Đã xóa truyện khỏi danh sách theo dõi."
                : result.Errors.Values.FirstOrDefault() ?? "Không thể xóa theo dõi.";

        return RedirectToAction(nameof(Following));
    }

    private int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : 0;
    }
}
