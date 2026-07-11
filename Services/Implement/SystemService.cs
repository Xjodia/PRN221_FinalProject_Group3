using PRN221_FinalProject_Group3.DAO;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Implement;

public class SystemService : ISystemService
{
    private readonly SystemDao _systemDao;

    public SystemService(SystemDao systemDao)
    {
        _systemDao = systemDao;
    }

    public async Task<SystemHomeViewModel> GetHomeAsync(CancellationToken cancellationToken = default)
    {
        return new SystemHomeViewModel
        {
            ActiveNovelCount = await _systemDao.CountNovelsAsync(true, cancellationToken),
            DeletedNovelCount = await _systemDao.CountNovelsAsync(false, cancellationToken),
            UserCount = await _systemDao.CountUsersAsync(cancellationToken),
            DeletedUserCount = await _systemDao.CountUsersAsync(UserStatus.Inactive, cancellationToken)
        };
    }

    public async Task<SystemNovelListViewModel> GetNovelsAsync(
        bool showDeleted,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var novels = await _systemDao.GetNovelsAsync(!showDeleted, searchTerm, cancellationToken);

        return new SystemNovelListViewModel
        {
            SearchTerm = searchTerm,
            ShowDeleted = showDeleted,
            Novels = novels.Select(novel => new SystemNovelItemViewModel
            {
                Id = novel.Id,
                Title = novel.Title,
                AuthorName = novel.Author.DisplayName,
                IsAuthorInactive = novel.Author.Status == UserStatus.Inactive,
                TranslationGroup = novel.TranslationGroup,
                ChapterCount = novel.Chapters.Count,
                UpdatedAtText = novel.UpdatedAt.ToLocalTime().ToString("dd/MM/yyyy")
            }).ToList()
        };
    }

    public async Task<SystemUserListViewModel> GetUsersAsync(
        bool showDeleted,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var users = await _systemDao.GetUsersAsync(showDeleted, searchTerm, cancellationToken);

        return new SystemUserListViewModel
        {
            SearchTerm = searchTerm,
            ShowDeleted = showDeleted,
            Users = users.Select(user => new SystemUserItemViewModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                IsInactive = user.Status == UserStatus.Inactive,
                Username = user.Username,
                Email = user.Email,
                Role = FormatRole(user.Role),
                Status = FormatStatus(user.Status),
                NovelCount = user.AuthoredNovels.Count,
                UpdatedAtText = user.UpdatedAt.ToLocalTime().ToString("dd/MM/yyyy")
            }).ToList()
        };
    }

    public async Task<SystemUserDetailViewModel?> GetUserDetailAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _systemDao.GetUserAsync(userId, cancellationToken: cancellationToken);
        if (user is null)
        {
            return null;
        }

        return new SystemUserDetailViewModel
        {
            User = MapUserEdit(user),
            Password = new SystemPasswordResetViewModel { UserId = user.Id }
        };
    }

    public async Task<ManagementResult> UpdateUserAsync(
        SystemUserEditViewModel model,
        int currentAdminId,
        CancellationToken cancellationToken = default)
    {
        var user = await _systemDao.GetUserAsync(
            model.Id,
            asTracking: true,
            cancellationToken);

        if (user is null)
        {
            return ManagementResult.Failure(string.Empty, "Không tìm thấy tài khoản.");
        }

        if (model.Id == currentAdminId
            && (model.Role != UserRole.Admin || model.Status != UserStatus.Active))
        {
            return ManagementResult.Failure(
                string.Empty,
                "Bạn không thể tự hạ quyền hoặc ngưng hoạt động tài khoản admin đang đăng nhập.");
        }

        if (await _systemDao.UsernameExistsForOtherUserAsync(
            model.Id,
            model.Username,
            cancellationToken))
        {
            return ManagementResult.Failure(nameof(model.Username), "Tên đăng nhập đã được sử dụng.");
        }

        if (await _systemDao.EmailExistsForOtherUserAsync(
            model.Id,
            model.Email,
            cancellationToken))
        {
            return ManagementResult.Failure(nameof(model.Email), "Email đã được sử dụng.");
        }

        user.Username = model.Username.Trim();
        user.Email = model.Email.Trim().ToLowerInvariant();
        user.DisplayName = model.DisplayName.Trim();
        user.Bio = string.IsNullOrWhiteSpace(model.Bio) ? null : model.Bio.Trim();
        user.AvatarUrl = string.IsNullOrWhiteSpace(model.AvatarUrl) ? null : model.AvatarUrl.Trim();
        user.Role = model.Role;
        user.Status = model.Status;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _systemDao.SaveChangesAsync(cancellationToken);
        return ManagementResult.Success(user.Id);
    }

    public async Task<ManagementResult> ResetUserPasswordAsync(
        SystemPasswordResetViewModel model,
        CancellationToken cancellationToken = default)
    {
        var user = await _systemDao.GetUserAsync(
            model.UserId,
            asTracking: true,
            cancellationToken);

        if (user is null)
        {
            return ManagementResult.Failure(string.Empty, "Không tìm thấy tài khoản.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _systemDao.SaveChangesAsync(cancellationToken);

        return ManagementResult.Success(user.Id);
    }

    public async Task<ManagementResult> SoftDeleteNovelAsync(
        int novelId,
        CancellationToken cancellationToken = default)
    {
        var novel = await _systemDao.GetNovelAsync(novelId, asTracking: true, cancellationToken);
        if (novel is null)
        {
            return ManagementResult.Failure(string.Empty, "Không tìm thấy truyện.");
        }

        novel.IsActive = false;
        novel.UpdatedAt = DateTimeOffset.UtcNow;
        await _systemDao.SaveChangesAsync(cancellationToken);

        return ManagementResult.Success(novel.Id);
    }

    public async Task<ManagementResult> RestoreNovelAsync(
        int novelId,
        CancellationToken cancellationToken = default)
    {
        var novel = await _systemDao.GetNovelAsync(novelId, asTracking: true, cancellationToken);
        if (novel is null)
        {
            return ManagementResult.Failure(string.Empty, "Không tìm thấy truyện.");
        }

        novel.IsActive = true;
        novel.UpdatedAt = DateTimeOffset.UtcNow;
        await _systemDao.SaveChangesAsync(cancellationToken);

        return ManagementResult.Success(novel.Id);
    }

    public async Task<ManagementResult> SoftDeleteUserAsync(
        int userId,
        int currentAdminId,
        CancellationToken cancellationToken = default)
    {
        if (userId == currentAdminId)
        {
            return ManagementResult.Failure(
                string.Empty,
                "Bạn không thể tự xóa mềm tài khoản admin đang đăng nhập.");
        }

        var user = await _systemDao.GetUserAsync(userId, asTracking: true, cancellationToken);
        if (user is null)
        {
            return ManagementResult.Failure(string.Empty, "Không tìm thấy tài khoản.");
        }

        user.Status = UserStatus.Inactive;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _systemDao.SaveChangesAsync(cancellationToken);

        return ManagementResult.Success(user.Id);
    }

    public async Task<ManagementResult> RestoreUserAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _systemDao.GetUserAsync(userId, asTracking: true, cancellationToken);
        if (user is null)
        {
            return ManagementResult.Failure(string.Empty, "Không tìm thấy tài khoản.");
        }

        user.Status = UserStatus.Active;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _systemDao.SaveChangesAsync(cancellationToken);

        return ManagementResult.Success(user.Id);
    }

    private static SystemUserEditViewModel MapUserEdit(User user)
    {
        return new SystemUserEditViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role,
            Status = user.Status,
            CreatedAtText = user.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            UpdatedAtText = user.UpdatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            NovelCount = user.AuthoredNovels.Count,
            ChapterCount = user.AuthoredNovels.Sum(novel => novel.Chapters.Count),
            CommentCount = user.ChapterComments.Count + user.NovelComments.Count
        };
    }

    private static string FormatRole(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => "Admin",
            UserRole.Moderator => "Moderator",
            UserRole.Author => "Tác giả",
            _ => "Độc giả"
        };
    }

    private static string FormatStatus(UserStatus status)
    {
        return status switch
        {
            UserStatus.Active => "Đang hoạt động",
            UserStatus.Suspended => "Tạm khóa",
            UserStatus.Banned => "Cấm",
            _ => "Đã ngưng hoạt động"
        };
    }
}
