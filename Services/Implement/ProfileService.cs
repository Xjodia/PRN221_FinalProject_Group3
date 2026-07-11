using PRN221_FinalProject_Group3.DAO;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;

namespace PRN221_FinalProject_Group3.Services.Implement;

public class ProfileService : IProfileService
{
    private const string DefaultCoverImage = "https://i.imgur.com/FTAaZvy.jpeg";

    private readonly ProfileDao _profileDao;

    public ProfileService(ProfileDao profileDao)
    {
        _profileDao = profileDao;
    }

    public async Task<ProfileViewModel?> GetPublicProfileAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _profileDao.GetPublicProfileAsync(userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var chapterCount = await _profileDao.CountAuthoredChaptersAsync(userId, cancellationToken);
        var commentCount = await _profileDao.CountCommentsAsync(userId, cancellationToken);
        var novels = user.AuthoredNovels
            .Where(novel => novel.IsActive)
            .OrderByDescending(novel => novel.UpdatedAt)
            .Select(novel => new ProfileNovelViewModel
            {
                Id = novel.Id,
                Title = novel.Title,
                CoverImage = string.IsNullOrWhiteSpace(novel.CoverImage)
                    ? DefaultCoverImage
                    : novel.CoverImage,
                StoryType = string.IsNullOrWhiteSpace(novel.StoryType)
                    ? "Truyện chữ"
                    : novel.StoryType,
                Status = FormatStatus(novel.Status),
                UpdatedText = FormatRelativeTime(novel.UpdatedAt),
                ChapterCount = novel.Chapters.Count
            })
            .ToList();

        return new ProfileViewModel
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Username = user.Username,
            Initials = BuildInitials(user.DisplayName),
            AvatarUrl = user.AvatarUrl,
            IsInactive = user.Status == UserStatus.Inactive,
            Bio = string.IsNullOrWhiteSpace(user.Bio)
                ? "Người dùng này chưa cập nhật giới thiệu."
                : user.Bio,
            JoinedText = user.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy"),
            NovelCount = novels.Count,
            ChapterCount = chapterCount,
            CommentCount = commentCount,
            Novels = novels
        };
    }

    private static string FormatStatus(NovelStatus status)
    {
        return status switch
        {
            NovelStatus.Completed => "Đã hoàn thành",
            NovelStatus.Paused => "Tạm dừng",
            _ => "Đang tiến hành"
        };
    }

    private static string FormatRelativeTime(DateTimeOffset value)
    {
        var span = DateTimeOffset.UtcNow - value.ToUniversalTime();

        if (span.TotalMinutes < 1)
        {
            return "vừa xong";
        }

        if (span.TotalHours < 1)
        {
            return $"{Math.Max(1, (int)span.TotalMinutes)} phút trước";
        }

        if (span.TotalDays < 1)
        {
            return $"{Math.Max(1, (int)span.TotalHours)} giờ trước";
        }

        if (span.TotalDays < 30)
        {
            return $"{Math.Max(1, (int)span.TotalDays)} ngày trước";
        }

        return value.ToLocalTime().ToString("dd/MM/yyyy");
    }

    private static string BuildInitials(string displayName)
    {
        var parts = displayName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0)
        {
            return "SN";
        }

        if (parts.Length == 1)
        {
            return parts[0][0].ToString().ToUpperInvariant();
        }

        return string.Concat(parts.TakeLast(2).Select(part => part[0])).ToUpperInvariant();
    }
}
