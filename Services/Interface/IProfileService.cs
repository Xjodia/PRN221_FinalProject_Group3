using PRN221_FinalProject_Group3.Models.ViewModels;

namespace PRN221_FinalProject_Group3.Services.Interface;

public interface IProfileService
{
    Task<ProfileViewModel?> GetPublicProfileAsync(
        int userId,
        CancellationToken cancellationToken = default);
}
