using Microsoft.AspNetCore.Mvc;
using PRN221_FinalProject_Group3.Services.Interface;

namespace PRN221_FinalProject_Group3.Controllers;

public class ProfileController : Controller
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    public async Task<IActionResult> Detail(
        int id,
        CancellationToken cancellationToken)
    {
        var viewModel = await _profileService.GetPublicProfileAsync(id, cancellationToken);

        if (viewModel is null)
        {
            return NotFound();
        }

        return View(viewModel);
    }
}
