using PRN221_FinalProject_Group3.DAO;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Implement;

public class DashboardService : IDashboardService
{
    private const string DefaultCoverImage = "https://i.imgur.com/FTAaZvy.jpeg";

    private readonly DashboardDao _dashboardDao;

    public DashboardService(DashboardDao dashboardDao)
    {
        _dashboardDao = dashboardDao;
    }

    public async Task<DashboardHomeViewModel> GetHomeAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _dashboardDao.GetUserAsync(userId, cancellationToken);
        var novels = await _dashboardDao.GetUserNovelsAsync(userId, cancellationToken: cancellationToken);
        var chapterCount = await _dashboardDao.CountUserChaptersAsync(userId, cancellationToken);

        return new DashboardHomeViewModel
        {
            DisplayName = user?.DisplayName ?? user?.Username ?? "bạn",
            NovelCount = novels.Count,
            ChapterCount = chapterCount
        };
    }

    public async Task<MyNovelListViewModel> GetMyNovelsAsync(
        int userId,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var novels = await _dashboardDao.GetUserNovelsAsync(userId, searchTerm, cancellationToken);

        return new MyNovelListViewModel
        {
            SearchTerm = searchTerm,
            Novels = novels.Select(novel => new MyNovelListItemViewModel
            {
                Id = novel.Id,
                Title = novel.Title,
                PublisherName = novel.Author.DisplayName,
                TranslationGroup = novel.TranslationGroup,
                ChapterCount = novel.Chapters.Count,
                UpdatedAtText = FormatDate(novel.UpdatedAt)
            }).ToList()
        };
    }

    public async Task<NovelFormViewModel> GetCreateNovelFormAsync(
        CancellationToken cancellationToken = default)
    {
        return new NovelFormViewModel
        {
            CategoryOptions = await GetCategoryOptionsAsync(cancellationToken),
            CoverImage = DefaultCoverImage
        };
    }

    public async Task<ManagementResult> CreateNovelAsync(
        NovelFormViewModel model,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!model.CategoryIds.Any())
        {
            return ManagementResult.Failure(nameof(model.CategoryIds), "Vui lòng chọn ít nhất một thể loại.");
        }

        var now = DateTimeOffset.UtcNow;
        var novel = new Novel
        {
            Title = model.Title.Trim(),
            AuthorId = userId,
            OtherNames = NormalizeOptional(model.OtherNames),
            OriginalAuthor = NormalizeOptional(model.OriginalAuthor),
            Illustrator = NormalizeOptional(model.Illustrator),
            StoryType = NormalizeOptional(model.StoryType) ?? "Truyện dịch",
            TranslationGroup = NormalizeOptional(model.TranslationGroup),
            Synopsis = model.Synopsis.Trim(),
            Note = NormalizeOptional(model.Note),
            CoverImage = NormalizeOptional(model.CoverImage) ?? DefaultCoverImage,
            Status = model.Status,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _dashboardDao.CreateNovelAsync(novel, model.CategoryIds, cancellationToken);
        return ManagementResult.Success(novel.Id);
    }

    public async Task<NovelFormViewModel?> GetEditNovelFormAsync(
        int novelId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var novel = await _dashboardDao.GetUserNovelDetailAsync(novelId, userId, cancellationToken: cancellationToken);
        if (novel is null)
        {
            return null;
        }

        return new NovelFormViewModel
        {
            Id = novel.Id,
            Title = novel.Title,
            OtherNames = novel.OtherNames,
            OriginalAuthor = novel.OriginalAuthor,
            Illustrator = novel.Illustrator,
            StoryType = novel.StoryType,
            TranslationGroup = novel.TranslationGroup,
            CoverImage = novel.CoverImage,
            Synopsis = novel.Synopsis,
            Note = novel.Note,
            Status = novel.Status,
            CategoryIds = novel.NovelCategories.Select(item => item.CategoryId).ToList(),
            CategoryOptions = await GetCategoryOptionsAsync(cancellationToken)
        };
    }

    public async Task<ManagementResult> UpdateNovelAsync(
        NovelFormViewModel model,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (model.Id is null)
        {
            return ManagementResult.Failure(string.Empty, "Không tìm thấy truyện cần cập nhật.");
        }

        if (!model.CategoryIds.Any())
        {
            return ManagementResult.Failure(nameof(model.CategoryIds), "Vui lòng chọn ít nhất một thể loại.");
        }

        var novel = await _dashboardDao.GetUserNovelDetailAsync(
            model.Id.Value,
            userId,
            asTracking: true,
            cancellationToken);

        if (novel is null)
        {
            return ManagementResult.Failure(string.Empty, "Bạn không có quyền sửa truyện này.");
        }

        novel.Title = model.Title.Trim();
        novel.OtherNames = NormalizeOptional(model.OtherNames);
        novel.OriginalAuthor = NormalizeOptional(model.OriginalAuthor);
        novel.Illustrator = NormalizeOptional(model.Illustrator);
        novel.StoryType = NormalizeOptional(model.StoryType) ?? "Truyện dịch";
        novel.TranslationGroup = NormalizeOptional(model.TranslationGroup);
        novel.CoverImage = NormalizeOptional(model.CoverImage) ?? DefaultCoverImage;
        novel.Synopsis = model.Synopsis.Trim();
        novel.Note = NormalizeOptional(model.Note);
        novel.Status = model.Status;
        novel.UpdatedAt = DateTimeOffset.UtcNow;

        await _dashboardDao.UpdateNovelCategoriesAsync(novel, model.CategoryIds, cancellationToken);
        return ManagementResult.Success(novel.Id);
    }

    public async Task<ManagedNovelDetailViewModel?> GetNovelDetailAsync(
        int novelId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var novel = await _dashboardDao.GetUserNovelDetailAsync(novelId, userId, cancellationToken: cancellationToken);
        if (novel is null)
        {
            return null;
        }

        return new ManagedNovelDetailViewModel
        {
            Id = novel.Id,
            Title = novel.Title,
            OriginalAuthor = novel.OriginalAuthor,
            Illustrator = novel.Illustrator,
            StoryType = novel.StoryType,
            TranslationGroup = novel.TranslationGroup,
            Synopsis = novel.Synopsis,
            Note = novel.Note,
            StatusText = FormatStatus(novel.Status),
            CoverImage = string.IsNullOrWhiteSpace(novel.CoverImage) ? DefaultCoverImage : novel.CoverImage,
            Chapters = novel.Chapters
                .OrderBy(chapter => chapter.ChapterNumber)
                .Select(chapter => new ManageChapterItemViewModel
                {
                    Id = chapter.Id,
                    Number = FormatChapterNumber(chapter.ChapterNumber),
                    Title = chapter.Title,
                    UpdatedAtText = FormatDate(chapter.UpdatedAt)
                }).ToList()
        };
    }

    public async Task<ManagementResult> DeleteNovelAsync(
        int novelId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var novel = await _dashboardDao.GetUserNovelDetailAsync(novelId, userId, asTracking: true, cancellationToken);
        if (novel is null)
        {
            return ManagementResult.Failure(string.Empty, "Bạn không có quyền xóa truyện này.");
        }

        await _dashboardDao.DeleteNovelAsync(novel, cancellationToken);
        return ManagementResult.Success();
    }

    public async Task<ChapterFormViewModel?> GetCreateChapterFormAsync(
        int novelId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var novel = await _dashboardDao.GetUserNovelDetailAsync(novelId, userId, cancellationToken: cancellationToken);
        if (novel is null)
        {
            return null;
        }

        var currentMax = await _dashboardDao.GetNextChapterNumberAsync(novelId, cancellationToken);
        return new ChapterFormViewModel
        {
            NovelId = novelId,
            ChapterNumber = currentMax + 1
        };
    }

    public async Task<ManagementResult> CreateChapterAsync(
        ChapterFormViewModel model,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var novel = await _dashboardDao.GetUserNovelDetailAsync(model.NovelId, userId, asTracking: true, cancellationToken);
        if (novel is null)
        {
            return ManagementResult.Failure(string.Empty, "Bạn không có quyền thêm chương cho truyện này.");
        }

        var chapterNumber = model.ChapterNumber <= 0
            ? await _dashboardDao.GetNextChapterNumberAsync(model.NovelId, cancellationToken) + 1
            : model.ChapterNumber;

        var now = DateTimeOffset.UtcNow;
        var chapter = new Chapter
        {
            NovelId = model.NovelId,
            ChapterNumber = chapterNumber,
            Title = model.Title.Trim(),
            Content = model.Content.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        novel.UpdatedAt = now;
        await _dashboardDao.CreateChapterAsync(chapter, cancellationToken);
        return ManagementResult.Success(chapter.Id);
    }

    public async Task<ChapterFormViewModel?> GetEditChapterFormAsync(
        int chapterId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var chapter = await _dashboardDao.GetUserChapterAsync(chapterId, userId, cancellationToken: cancellationToken);
        if (chapter is null)
        {
            return null;
        }

        return new ChapterFormViewModel
        {
            Id = chapter.Id,
            NovelId = chapter.NovelId,
            ChapterNumber = chapter.ChapterNumber,
            Title = chapter.Title,
            Content = chapter.Content
        };
    }

    public async Task<ManagementResult> UpdateChapterAsync(
        ChapterFormViewModel model,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (model.Id is null)
        {
            return ManagementResult.Failure(string.Empty, "Không tìm thấy chương cần cập nhật.");
        }

        var chapter = await _dashboardDao.GetUserChapterAsync(
            model.Id.Value,
            userId,
            asTracking: true,
            cancellationToken);

        if (chapter is null)
        {
            return ManagementResult.Failure(string.Empty, "Bạn không có quyền sửa chương này.");
        }

        chapter.ChapterNumber = model.ChapterNumber <= 0 ? chapter.ChapterNumber : model.ChapterNumber;
        chapter.Title = model.Title.Trim();
        chapter.Content = model.Content.Trim();
        chapter.UpdatedAt = DateTimeOffset.UtcNow;
        chapter.Novel.UpdatedAt = chapter.UpdatedAt;

        await _dashboardDao.SaveChangesAsync(cancellationToken);
        return ManagementResult.Success(chapter.Id);
    }

    public async Task<ManagementResult> DeleteChapterAsync(
        int chapterId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var chapter = await _dashboardDao.GetUserChapterAsync(chapterId, userId, asTracking: true, cancellationToken);
        if (chapter is null)
        {
            return ManagementResult.Failure(string.Empty, "Bạn không có quyền xóa chương này.");
        }

        var novelId = chapter.NovelId;
        await _dashboardDao.DeleteChapterAsync(chapter, cancellationToken);
        return ManagementResult.Success(novelId);
    }

    private async Task<IReadOnlyList<CategoryOptionViewModel>> GetCategoryOptionsAsync(
        CancellationToken cancellationToken)
    {
        var categories = await _dashboardDao.GetCategoriesAsync(cancellationToken);
        return categories
            .Select(category => new CategoryOptionViewModel
            {
                Id = category.Id,
                Name = category.Name
            })
            .ToList();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string FormatDate(DateTimeOffset date)
    {
        return date.ToLocalTime().ToString("dd/MM/yyyy");
    }

    private static string FormatChapterNumber(decimal number)
    {
        return number % 1 == 0 ? ((int)number).ToString() : number.ToString("0.##");
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
}
