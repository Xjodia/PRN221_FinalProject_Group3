using PRN221_FinalProject_Group3.Models.ViewModels;

namespace PRN221_FinalProject_Group3.Services.Interface;

public interface INovelService
{
    Task<HomeViewModel> GetHomePageAsync(CancellationToken cancellationToken = default);

    Task<NovelDetailViewModel?> GetNovelDetailAsync(
        int id,
        CancellationToken cancellationToken = default);
}
