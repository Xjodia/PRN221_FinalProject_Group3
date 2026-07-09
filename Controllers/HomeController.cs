using Microsoft.AspNetCore.Mvc;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Services.Interface;
using System.Diagnostics;

namespace PRN221_FinalProject_Group3.Controllers;

public class HomeController : Controller
{
    private readonly INovelService _novelService;

    public HomeController(INovelService novelService)
    {
        _novelService = novelService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = await _novelService.GetHomePageAsync(cancellationToken);
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
