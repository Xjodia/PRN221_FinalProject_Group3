using PRN221_FinalProject_Group3.DAO;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Implement;

public class LibraryService : ILibraryService
{
    private const string DefaultCoverImage = "https://i.imgur.com/FTAaZvy.jpeg";

    private readonly LibraryDao _libraryDao;

    public LibraryService(LibraryDao libraryDao)
    {
        _libraryDao = libraryDao;
    }

    public async Task<FollowedNovelsViewModel> GetFollowedNovelsAsync(
        int userId,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var follows = await _libraryDao.GetFollowedNovelsAsync(
            userId,
            searchTerm,
            cancellationToken);

        return new FollowedNovelsViewModel
        {
            SearchTerm = searchTerm,
            Novels = follows.Select(follow =>
            {
                var latestChapter = follow.Novel.Chapters
                    .OrderByDescending(chapter => chapter.ChapterNumber)
                    .FirstOrDefault();

                return new FollowedNovelItemViewModel
                {
                    NovelId = follow.NovelId,
                    Title = follow.Novel.Title,
                    AuthorName = follow.Novel.Author.DisplayName,
                    CoverImage = GetCoverImage(follow.Novel.CoverImage),
                    Status = FormatStatus(follow.Novel.Status),
                    LatestChapterId = latestChapter?.Id,
                    LatestChapterTitle = latestChapter is null
                        ? "Chưa có chương"
                        : $"Chương {FormatChapterNumber(latestChapter.ChapterNumber)}: {latestChapter.Title}",
                    FollowedAtText = FormatRelativeTime(follow.CreatedAt)
                };
            }).ToList()
        };
    }

    public async Task<ReadingHistoryViewModel> GetReadingHistoryAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var histories = await _libraryDao.GetReadingHistoriesAsync(userId, cancellationToken);

        return new ReadingHistoryViewModel
        {
            Items = histories.Select(history => new ReadingHistoryItemViewModel
            {
                NovelId = history.NovelId,
                NovelTitle = history.Novel.Title,
                ChapterId = history.ChapterId,
                ChapterTitle = $"Chương {FormatChapterNumber(history.Chapter.ChapterNumber)}: {history.Chapter.Title}",
                LastReadAtText = FormatRelativeTime(history.LastReadAt)
            }).ToList()
        };
    }

    public Task<bool> IsFollowingAsync(
        int userId,
        int novelId,
        CancellationToken cancellationToken = default)
    {
        return _libraryDao.IsFollowingAsync(userId, novelId, cancellationToken);
    }

    public async Task<ManagementResult> ToggleFollowAsync(
        int userId,
        int novelId,
        CancellationToken cancellationToken = default)
    {
        if (!await _libraryDao.ActiveNovelExistsAsync(novelId, cancellationToken))
        {
            return ManagementResult.Failure(string.Empty, "Không tìm thấy truyện.");
        }

        var follow = await _libraryDao.GetFollowAsync(
            userId,
            novelId,
            asTracking: true,
            cancellationToken);

        if (follow is not null)
        {
            await _libraryDao.RemoveFollowAsync(follow, cancellationToken);
            return ManagementResult.Success(novelId);
        }

        await _libraryDao.AddFollowAsync(
            new Follow
            {
                UserId = userId,
                NovelId = novelId,
                CreatedAt = DateTimeOffset.UtcNow
            },
            cancellationToken);

        return ManagementResult.Success(novelId);
    }

    public async Task<ManagementResult> RemoveFollowAsync(
        int userId,
        int novelId,
        CancellationToken cancellationToken = default)
    {
        var follow = await _libraryDao.GetFollowAsync(
            userId,
            novelId,
            asTracking: true,
            cancellationToken);

        if (follow is null)
        {
            return ManagementResult.Failure(string.Empty, "Bạn chưa theo dõi truyện này.");
        }

        await _libraryDao.RemoveFollowAsync(follow, cancellationToken);
        return ManagementResult.Success(novelId);
    }

    public async Task SaveReadingHistoryAsync(
        int userId,
        int chapterId,
        CancellationToken cancellationToken = default)
    {
        var chapter = await _libraryDao.GetActiveChapterAsync(chapterId, cancellationToken);
        if (chapter is null)
        {
            return;
        }

        var history = await _libraryDao.GetReadingHistoryAsync(
            userId,
            chapter.NovelId,
            asTracking: true,
            cancellationToken);

        var isNew = history is null;
        history ??= new ReadingHistory
        {
            UserId = userId,
            NovelId = chapter.NovelId
        };

        history.ChapterId = chapter.Id;
        history.LastReadAt = DateTimeOffset.UtcNow;

        await _libraryDao.SaveReadingHistoryAsync(history, isNew, cancellationToken);
    }

    private static string GetCoverImage(string? coverImage)
    {
        return string.IsNullOrWhiteSpace(coverImage)
            ? DefaultCoverImage
            : coverImage;
    }

    private static string FormatStatus(NovelStatus status)
    {
        return status switch
        {
            NovelStatus.Completed => "Hoàn thành",
            NovelStatus.Paused => "Tạm dừng",
            _ => "Đang tiến hành"
        };
    }

    private static string FormatChapterNumber(decimal number)
    {
        return number % 1 == 0
            ? ((int)number).ToString()
            : number.ToString("0.##");
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
}
