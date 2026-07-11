using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Interface;

public interface ISystemService
{
    Task<SystemHomeViewModel> GetHomeAsync(CancellationToken cancellationToken = default);

    Task<SystemNovelListViewModel> GetNovelsAsync(
        bool showDeleted,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> SoftDeleteNovelAsync(
        int novelId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> RestoreNovelAsync(
        int novelId,
        CancellationToken cancellationToken = default);
}
