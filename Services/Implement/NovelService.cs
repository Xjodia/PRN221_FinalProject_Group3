using PRN221_FinalProject_Group3.DAO;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;

namespace PRN221_FinalProject_Group3.Services.Implement;

public class NovelService : INovelService
{
    private const string DefaultCoverImage = "https://i.imgur.com/FTAaZvy.jpeg";

    private readonly NovelDao _novelDao;

    public NovelService(NovelDao novelDao)
    {
        _novelDao = novelDao;
    }

    public async Task<HomeViewModel> GetHomePageAsync(
        CancellationToken cancellationToken = default)
    {
        var featuredNovels = await _novelDao.GetFeaturedNovelsAsync(cancellationToken: cancellationToken);
        var latestChapters = await _novelDao.GetLatestChaptersAsync(cancellationToken: cancellationToken);
        var popularNovels = await _novelDao.GetPopularNovelsAsync(cancellationToken: cancellationToken);
        var categories = await _novelDao.GetCategoriesAsync(cancellationToken: cancellationToken);

        return new HomeViewModel
        {
            FeaturedNovels = featuredNovels
                .Select(MapNovelCard)
                .ToList(),
            LatestChapters = latestChapters
                .Select(MapLatestChapter)
                .ToList(),
            PopularNovels = popularNovels
                .Select((novel, index) => new PopularNovelViewModel
                {
                    Id = novel.Id,
                    Rank = index + 1,
                    Title = novel.Title,
                    Author = novel.Author.DisplayName,
                    ViewCount = novel.ViewCount
                })
                .ToList(),
            Categories = categories
                .Select(category => category.Name)
                .ToList()
        };
    }

    public async Task<NovelDetailViewModel?> GetNovelDetailAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var novel = await _novelDao.GetDetailAsync(id, cancellationToken);

        if (novel is null)
        {
            return null;
        }

        var recommendedNovels = await _novelDao.GetRecommendedNovelsAsync(
            novel.Id,
            cancellationToken: cancellationToken);

        var orderedChapters = novel.Chapters
            .OrderBy(chapter => chapter.ChapterNumber)
            .ToList();
        var firstChapter = orderedChapters.FirstOrDefault();
        var comments = orderedChapters
            .SelectMany(chapter => chapter.Comments)
            .OrderByDescending(comment => comment.CreatedAt)
            .Take(8)
            .ToList();

        return new NovelDetailViewModel
        {
            Id = novel.Id,
            Title = novel.Title,
            AuthorName = novel.Author.DisplayName,
            TypeName = "Truyện Sáng Tác",
            Status = FormatStatus(novel.Status),
            UpdatedAtText = FormatDate(novel.UpdatedAt),
            Synopsis = novel.Synopsis,
            CoverImage = GetCoverImage(novel.CoverImage),
            FirstChapterId = firstChapter?.Id,
            Categories = novel.NovelCategories
                .Select(item => item.Category.Name)
                .OrderBy(name => name)
                .ToList(),
            Statistics =
            [
                new() { Value = FormatShortNumber(EstimateWordCount(novel)), Label = "Số từ" },
                new() { Value = FormatShortNumber(novel.ViewCount), Label = "Lượt xem" },
                new() { Value = FormatShortNumber(comments.Count), Label = "Bình luận" },
                new() { Value = FormatShortNumber(novel.Followers.Count), Label = "Theo dõi" },
                new() { Value = "1", Label = "Đề cử", IsHighlight = true },
                new() { Value = "—★", Label = "Đánh giá" }
            ],
            Chapters = orderedChapters
                .Select(chapter => new NovelChapterItemViewModel
                {
                    Id = chapter.Id,
                    Number = FormatChapterNumber(chapter.ChapterNumber),
                    Title = chapter.Title,
                    UpdatedAtText = chapter.UpdatedAt.ToString("dd/MM/yyyy"),
                    IsNew = chapter == firstChapter
                })
                .ToList(),
            Author = new AuthorCardViewModel
            {
                DisplayName = novel.Author.DisplayName,
                Initials = BuildInitials(novel.Author.DisplayName),
                Level = novel.Author.Role == UserRole.Author ? "Author" : "Lv.1",
                JoinedText = $"Thành viên từ {novel.Author.CreatedAt:yyyy}",
                Bio = string.IsNullOrWhiteSpace(novel.Author.Bio)
                    ? "Tác giả của những câu chuyện đang được đăng tải trên StoryNest."
                    : novel.Author.Bio
            },
            RecommendedNovels = recommendedNovels
                .Select(item => new RecommendedNovelViewModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Author = item.Author.DisplayName,
                    CoverImage = GetCoverImage(item.CoverImage),
                    RatingText = "Chưa có"
                })
                .ToList(),
            Comments = comments
                .Select(comment => new CommentViewModel
                {
                    UserName = comment.User.DisplayName,
                    Initials = BuildInitials(comment.User.DisplayName),
                    Level = comment.User.Role == UserRole.Author ? "Author" : "Lv.1",
                    Content = comment.Content,
                    CreatedAtText = FormatRelativeTime(comment.CreatedAt),
                    IsPinned = comment.UserId == novel.AuthorId
                })
                .ToList()
        };
    }

    private static NovelCardViewModel MapNovelCard(Novel novel)
    {
        var latestChapter = novel.Chapters
            .OrderByDescending(chapter => chapter.ChapterNumber)
            .FirstOrDefault();

        return new NovelCardViewModel
        {
            Id = novel.Id,
            Title = novel.Title,
            Author = novel.Author.DisplayName,
            LatestChapter = latestChapter is null
                ? "Chưa có chương"
                : $"Chương {FormatChapterNumber(latestChapter.ChapterNumber)}: {latestChapter.Title}",
            Status = FormatStatus(novel.Status),
            CoverImage = GetCoverImage(novel.CoverImage),
            ViewCount = novel.ViewCount
        };
    }

    private static LatestChapterViewModel MapLatestChapter(Chapter chapter)
    {
        return new LatestChapterViewModel
        {
            NovelId = chapter.NovelId,
            ChapterId = chapter.Id,
            NovelTitle = chapter.Novel.Title,
            ChapterTitle = $"Chương {FormatChapterNumber(chapter.ChapterNumber)}: {chapter.Title}",
            Category = chapter.Novel.NovelCategories
                .Select(item => item.Category.Name)
                .FirstOrDefault() ?? "Truyện chữ",
            UpdatedText = FormatRelativeTime(chapter.UpdatedAt)
        };
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

    private static string FormatChapterNumber(decimal chapterNumber)
    {
        return chapterNumber % 1 == 0
            ? chapterNumber.ToString("0")
            : chapterNumber.ToString("0.##");
    }

    private static string FormatDate(DateTimeOffset value)
    {
        return $"{value.Day} tháng {value.Month}, {value.Year}";
    }

    private static string FormatShortNumber(long value)
    {
        if (value >= 1_000_000)
        {
            return $"{value / 1_000_000D:0.#}M";
        }

        if (value >= 1_000)
        {
            return $"{value / 1_000D:0.#}K";
        }

        return value.ToString("N0");
    }

    private static long EstimateWordCount(Novel novel)
    {
        return novel.Chapters.Sum(chapter =>
            chapter.Content.Split(
                [' ', '\r', '\n', '\t'],
                StringSplitOptions.RemoveEmptyEntries).LongLength);
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

        return value.ToString("dd/MM/yyyy");
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
