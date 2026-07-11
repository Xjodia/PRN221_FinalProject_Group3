using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Interface;

public interface IDashboardService
{
    Task<DashboardHomeViewModel> GetHomeAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<MyNovelListViewModel> GetMyNovelsAsync(
        int userId,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<NovelFormViewModel> GetCreateNovelFormAsync(
        CancellationToken cancellationToken = default);

    Task<ManagementResult> CreateNovelAsync(
        NovelFormViewModel model,
        int userId,
        CancellationToken cancellationToken = default);

    Task<NovelFormViewModel?> GetEditNovelFormAsync(
        int novelId,
        int userId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> UpdateNovelAsync(
        NovelFormViewModel model,
        int userId,
        CancellationToken cancellationToken = default);

    Task<ManagedNovelDetailViewModel?> GetNovelDetailAsync(
        int novelId,
        int userId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> DeleteNovelAsync(
        int novelId,
        int userId,
        CancellationToken cancellationToken = default);

    Task<ChapterFormViewModel?> GetCreateChapterFormAsync(
        int novelId,
        int userId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> CreateChapterAsync(
        ChapterFormViewModel model,
        int userId,
        CancellationToken cancellationToken = default);

    Task<ChapterFormViewModel?> GetEditChapterFormAsync(
        int chapterId,
        int userId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> UpdateChapterAsync(
        ChapterFormViewModel model,
        int userId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> DeleteChapterAsync(
        int chapterId,
        int userId,
        CancellationToken cancellationToken = default);
}
