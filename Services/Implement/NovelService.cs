using System.Text.RegularExpressions;
using PRN221_FinalProject_Group3.DAO;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Implement;

public class NovelService : INovelService
{
    private const string DefaultCoverImage = "https://i.imgur.com/FTAaZvy.jpeg";

    private readonly NovelDao _novelDao;
    private readonly LibraryDao _libraryDao;

    public NovelService(
        NovelDao novelDao,
        LibraryDao libraryDao)
    {
        _novelDao = novelDao;
        _libraryDao = libraryDao;
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
        int? currentUserId = null,
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
        var comments = novel.Comments
            .OrderByDescending(comment => comment.CreatedAt)
            .ToList();
        var commentLookup = novel.Comments
            .OrderBy(comment => comment.CreatedAt)
            .GroupBy(comment => comment.ParentCommentId ?? 0)
            .ToDictionary(group => group.Key, group => group.ToList());
        var isFollowed = currentUserId.HasValue
                          && await _libraryDao.IsFollowingAsync(
                              currentUserId.Value,
                              novel.Id,
                              cancellationToken);

        return new NovelDetailViewModel
        {
            Id = novel.Id,
            Title = novel.Title,
            AuthorName = novel.Author.DisplayName,
            AuthorId = novel.AuthorId,
            IsAuthorInactive = novel.Author.Status == UserStatus.Inactive,
            TypeName = "Truyện Sáng Tác",
            Status = FormatStatus(novel.Status),
            UpdatedAtText = FormatDate(novel.UpdatedAt),
            Synopsis = novel.Synopsis,
            CoverImage = GetCoverImage(novel.CoverImage),
            FirstChapterId = firstChapter?.Id,
            IsFollowed = isFollowed,
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
                Id = novel.Author.Id,
                DisplayName = novel.Author.DisplayName,
                IsInactive = novel.Author.Status == UserStatus.Inactive,
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
                    IsAuthorInactive = item.Author.Status == UserStatus.Inactive,
                    CoverImage = GetCoverImage(item.CoverImage),
                    RatingText = "Chưa có"
                })
                .ToList(),
            Comments = comments
                .Where(comment => comment.ParentCommentId is null)
                .OrderBy(comment => comment.CreatedAt)
                .Select(comment => MapComment(comment, commentLookup, novel.AuthorId))
                .ToList()
        };
    }

    public async Task<SearchViewModel> SearchAsync(
        string? query,
        string? tab = null,
        string? status = null,
        string? sort = null,
        CancellationToken cancellationToken = default)
    {
        var keyword = (query ?? string.Empty).Trim();
        var normalizedTab = NormalizeSearchTab(tab);
        var normalizedStatus = NormalizeSearchStatus(status);
        var normalizedSort = NormalizeSearchSort(sort);

        if (keyword.Length is > 0 and < 2)
        {
            return new SearchViewModel
            {
                Query = keyword,
                Tab = normalizedTab,
                Status = normalizedStatus,
                Sort = normalizedSort
            };
        }

        var shouldLoadNovels = normalizedTab is "all" or "novels";
        var shouldLoadMembers = keyword.Length >= 2
                                && (normalizedTab is "all" or "members");

        var novels = shouldLoadNovels
            ? await _novelDao.SearchNovelsAsync(
                keyword,
                ParseNovelStatus(normalizedStatus),
                normalizedSort,
                cancellationToken: cancellationToken)
            : [];

        var members = shouldLoadMembers
            ? await _novelDao.SearchMembersAsync(keyword, cancellationToken: cancellationToken)
            : [];

        return new SearchViewModel
        {
            Query = keyword,
            Tab = normalizedTab,
            Status = normalizedStatus,
            Sort = normalizedSort,
            NovelResults = novels.Select(MapSearchNovel).ToList(),
            MemberResults = members.Select(MapSearchMember).ToList()
        };
    }

    public async Task<CommentResult> AddCommentAsync(
        NovelCommentInputViewModel model,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _novelDao.NovelExistsAsync(model.NovelId, cancellationToken))
        {
            return CommentResult.Failure("Truyện không tồn tại.");
        }

        if (model.ParentCommentId.HasValue
            && !await _novelDao.CommentBelongsToNovelAsync(
                model.ParentCommentId.Value,
                model.NovelId,
                cancellationToken))
        {
            return CommentResult.Failure("Bình luận gốc không hợp lệ.");
        }

        var sanitizedContent = SanitizeTinyMceHtml(model.Content);

        if (string.IsNullOrWhiteSpace(StripHtml(sanitizedContent)))
        {
            return CommentResult.Failure("Vui lòng nhập nội dung bình luận.");
        }

        var now = DateTimeOffset.UtcNow;
        var comment = new NovelComment
        {
            NovelId = model.NovelId,
            UserId = userId,
            ParentCommentId = model.ParentCommentId,
            Content = sanitizedContent,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _novelDao.CreateCommentAsync(comment, cancellationToken);
        return CommentResult.Success();
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

    private static CommentViewModel MapComment(
        NovelComment comment,
        IReadOnlyDictionary<int, List<NovelComment>> lookup,
        int ownerUserId)
    {
        lookup.TryGetValue(comment.Id, out var replies);

        return new CommentViewModel
        {
            Id = comment.Id,
            NovelId = comment.NovelId,
            UserId = comment.UserId,
            UserName = comment.User.DisplayName,
            IsUserInactive = comment.User.Status == UserStatus.Inactive,
            Initials = BuildInitials(comment.User.DisplayName),
            Level = comment.User.Role == UserRole.Author ? "Author" : "Lv.1",
            Content = comment.Content,
            CreatedAtText = FormatRelativeTime(comment.CreatedAt),
            IsPinned = comment.UserId == ownerUserId,
            Replies = replies is null
                ? []
                : replies.Select(reply => MapComment(reply, lookup, ownerUserId)).ToList()
        };
    }

    private static string SanitizeTinyMceHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var sanitized = html.Trim();
        sanitized = Regex.Replace(
            sanitized,
            @"<\s*(script|style|iframe|object|embed|form)[^>]*>.*?<\s*/\s*\1\s*>",
            string.Empty,
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        sanitized = Regex.Replace(
            sanitized,
            @"\s+on[a-z]+\s*=\s*(['""]).*?\1",
            string.Empty,
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        sanitized = Regex.Replace(
            sanitized,
            @"\s+on[a-z]+\s*=\s*[^\s>]+",
            string.Empty,
            RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(
            sanitized,
            @"(href|src)\s*=\s*(['""])\s*javascript:.*?\2",
            "$1=\"#\"",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        return sanitized;
    }

    private static string StripHtml(string html)
    {
        return Regex.Replace(html, "<.*?>", string.Empty, RegexOptions.Singleline)
            .Replace("&nbsp;", " ")
            .Trim();
    }

    private static string GetCoverImage(string? coverImage)
    {
        return string.IsNullOrWhiteSpace(coverImage)
            ? DefaultCoverImage
            : coverImage;
    }

    private static SearchNovelResultViewModel MapSearchNovel(Novel novel)
    {
        return new SearchNovelResultViewModel
        {
            Id = novel.Id,
            Title = novel.Title,
            Author = novel.Author.DisplayName,
            AuthorId = novel.AuthorId,
            IsAuthorInactive = novel.Author.Status == UserStatus.Inactive,
            Synopsis = BuildSearchSynopsis(novel.Synopsis),
            CoverImage = GetCoverImage(novel.CoverImage),
            Status = FormatStatus(novel.Status),
            Category = novel.NovelCategories
                .Select(item => item.Category.Name)
                .OrderBy(name => name)
                .FirstOrDefault() ?? "Truyện chữ",
            ViewCount = novel.ViewCount,
            ChapterCount = novel.Chapters.Count,
            UpdatedText = FormatRelativeTime(novel.UpdatedAt)
        };
    }

    private static SearchMemberResultViewModel MapSearchMember(User user)
    {
        return new SearchMemberResultViewModel
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            IsInactive = user.Status == UserStatus.Inactive,
            Username = user.Username,
            Initials = BuildInitials(user.DisplayName),
            Role = user.Role == UserRole.Author ? "Tác giả" : "Thành viên",
            Bio = string.IsNullOrWhiteSpace(user.Bio)
                ? "Chưa có giới thiệu."
                : user.Bio,
            NovelCount = user.AuthoredNovels.Count
        };
    }

    private static string BuildSearchSynopsis(string synopsis)
    {
        var text = StripHtml(synopsis)
            .Replace("&nbsp;", " ")
            .Replace("&amp;", "&");

        return text.Length <= 260
            ? text
            : $"{text[..260]}...";
    }

    private static string NormalizeSearchTab(string? tab)
    {
        return tab?.Trim().ToLowerInvariant() switch
        {
            "novels" => "novels",
            "members" => "members",
            _ => "all"
        };
    }

    private static string NormalizeSearchStatus(string? status)
    {
        return status?.Trim().ToLowerInvariant() switch
        {
            "ongoing" => "ongoing",
            "completed" => "completed",
            "paused" => "paused",
            _ => "all"
        };
    }

    private static string NormalizeSearchSort(string? sort)
    {
        return sort?.Trim().ToLowerInvariant() switch
        {
            "az" => "az",
            "latest" => "latest",
            "popular" => "popular",
            "relevance" => "relevance",
            _ => "az"
        };
    }

    private static NovelStatus? ParseNovelStatus(string status)
    {
        return status switch
        {
            "ongoing" => NovelStatus.Ongoing,
            "completed" => NovelStatus.Completed,
            "paused" => NovelStatus.Paused,
            _ => null
        };
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
