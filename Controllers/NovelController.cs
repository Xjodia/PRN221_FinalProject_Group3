using Microsoft.AspNetCore.Mvc;
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
}
