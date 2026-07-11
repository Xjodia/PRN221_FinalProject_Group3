using PRN221_FinalProject_Group3.DAO;
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
            UserCount = await _systemDao.CountUsersAsync(cancellationToken)
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
                TranslationGroup = novel.TranslationGroup,
                ChapterCount = novel.Chapters.Count,
                UpdatedAtText = novel.UpdatedAt.ToLocalTime().ToString("dd/MM/yyyy")
            }).ToList()
        };
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
}
