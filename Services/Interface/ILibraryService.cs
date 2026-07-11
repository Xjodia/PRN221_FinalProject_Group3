using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Interface;

public interface ILibraryService
{
    Task<FollowedNovelsViewModel> GetFollowedNovelsAsync(
        int userId,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<ReadingHistoryViewModel> GetReadingHistoryAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<bool> IsFollowingAsync(
        int userId,
        int novelId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> ToggleFollowAsync(
        int userId,
        int novelId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> RemoveFollowAsync(
        int userId,
        int novelId,
        CancellationToken cancellationToken = default);

    Task SaveReadingHistoryAsync(
        int userId,
        int chapterId,
        CancellationToken cancellationToken = default);
}
