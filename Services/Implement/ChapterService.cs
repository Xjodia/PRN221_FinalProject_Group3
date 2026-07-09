using System.Text.RegularExpressions;
using PRN221_FinalProject_Group3.DAO;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Implement;

public class ChapterService : IChapterService
{
    private readonly ChapterDao _chapterDao;

    public ChapterService(ChapterDao chapterDao)
    {
        _chapterDao = chapterDao;
    }

    public async Task<ChapterDetailViewModel?> GetChapterDetailAsync(
        int chapterId,
        CancellationToken cancellationToken = default)
    {
        var chapter = await _chapterDao.GetDetailAsync(chapterId, cancellationToken);

        if (chapter is null)
        {
            return null;
        }

        var previousChapter = await _chapterDao.GetPreviousChapterAsync(
            chapter.NovelId,
            chapter.ChapterNumber,
            cancellationToken);
        var nextChapter = await _chapterDao.GetNextChapterAsync(
            chapter.NovelId,
            chapter.ChapterNumber,
            cancellationToken);

        var orderedComments = chapter.Comments
            .OrderBy(comment => comment.CreatedAt)
            .ToList();
        var commentLookup = orderedComments
            .GroupBy(comment => comment.ParentCommentId ?? 0)
            .ToDictionary(group => group.Key, group => group.ToList());

        return new ChapterDetailViewModel
        {
            NovelId = chapter.NovelId,
            NovelTitle = chapter.Novel.Title,
            ChapterId = chapter.Id,
            ChapterNumber = FormatChapterNumber(chapter.ChapterNumber),
            ChapterTitle = chapter.Title,
            Content = chapter.Content,
            UpdatedAtText = FormatRelativeTime(chapter.UpdatedAt),
            WordCountText = FormatShortNumber(EstimateWordCount(chapter.Content)),
            CommentCount = orderedComments.Count,
            PreviousChapterId = previousChapter?.Id,
            NextChapterId = nextChapter?.Id,
            Comments = BuildCommentTree(
                0,
                commentLookup,
                chapter.Id,
                chapter.Novel.AuthorId)
        };
    }

    public async Task<CommentResult> AddCommentAsync(
        ChapterCommentInputViewModel model,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _chapterDao.ChapterExistsAsync(model.ChapterId, cancellationToken))
        {
            return CommentResult.Failure("Chương không tồn tại.");
        }

        if (model.ParentCommentId.HasValue
            && !await _chapterDao.CommentBelongsToChapterAsync(
                model.ParentCommentId.Value,
                model.ChapterId,
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
        var comment = new ChapterComment
        {
            ChapterId = model.ChapterId,
            UserId = userId,
            ParentCommentId = model.ParentCommentId,
            Content = sanitizedContent,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _chapterDao.CreateCommentAsync(comment, cancellationToken);
        return CommentResult.Success();
    }

    private static IReadOnlyList<ChapterCommentItemViewModel> BuildCommentTree(
        int parentCommentId,
        IReadOnlyDictionary<int, List<ChapterComment>> lookup,
        int chapterId,
        int ownerUserId)
    {
        if (!lookup.TryGetValue(parentCommentId, out var comments))
        {
            return [];
        }

        return comments
            .Select(comment => new ChapterCommentItemViewModel
            {
                Id = comment.Id,
                ChapterId = chapterId,
                UserName = comment.User.DisplayName,
                Initials = BuildInitials(comment.User.DisplayName),
                Level = comment.User.Role == UserRole.Author ? "TRANS" : "Lv.1",
                Content = comment.Content,
                CreatedAtText = FormatRelativeTime(comment.CreatedAt),
                IsOwner = comment.UserId == ownerUserId,
                Replies = BuildCommentTree(comment.Id, lookup, chapterId, ownerUserId)
            })
            .ToList();
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

    private static long EstimateWordCount(string html)
    {
        var text = StripHtml(html);
        return text.Split(
            [' ', '\r', '\n', '\t'],
            StringSplitOptions.RemoveEmptyEntries).LongLength;
    }

    private static string FormatChapterNumber(decimal chapterNumber)
    {
        return chapterNumber % 1 == 0
            ? chapterNumber.ToString("0")
            : chapterNumber.ToString("0.##");
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
