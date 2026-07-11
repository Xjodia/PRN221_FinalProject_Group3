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

    Task<SystemUserListViewModel> GetUsersAsync(
        bool showDeleted,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<SystemUserDetailViewModel?> GetUserDetailAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> UpdateUserAsync(
        SystemUserEditViewModel model,
        int currentAdminId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> ResetUserPasswordAsync(
        SystemPasswordResetViewModel model,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> SoftDeleteNovelAsync(
        int novelId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> RestoreNovelAsync(
        int novelId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> SoftDeleteUserAsync(
        int userId,
        int currentAdminId,
        CancellationToken cancellationToken = default);

    Task<ManagementResult> RestoreUserAsync(
        int userId,
        CancellationToken cancellationToken = default);
}
