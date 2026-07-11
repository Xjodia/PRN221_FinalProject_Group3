using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.Data;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.DAO;

public class DashboardDao
{
    private readonly ApplicationDbContext _context;

    public DashboardDao(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetUserAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Novel>> GetUserNovelsAsync(
        int userId,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Novels
            .AsNoTracking()
            .Include(novel => novel.Author)
            .Include(novel => novel.Chapters)
            .Where(novel => novel.AuthorId == userId && novel.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim().ToLower();
            query = query.Where(novel => novel.Title.ToLower().Contains(keyword));
        }

        return query
            .OrderByDescending(novel => novel.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountUserChaptersAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Chapters
            .Where(chapter => chapter.Novel.AuthorId == userId && chapter.Novel.IsActive)
            .CountAsync(cancellationToken);
    }

    public Task<Novel?> GetUserNovelDetailAsync(
        int novelId,
        int userId,
        bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Novels
            .Include(novel => novel.Author)
            .Include(novel => novel.Chapters)
            .Include(novel => novel.NovelCategories)
                .ThenInclude(item => item.Category)
            .Where(novel => novel.Id == novelId && novel.AuthorId == userId && novel.IsActive);

        if (!asTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Novel> CreateNovelAsync(
        Novel novel,
        IEnumerable<int> categoryIds,
        CancellationToken cancellationToken = default)
    {
        foreach (var categoryId in categoryIds.Distinct())
        {
            novel.NovelCategories.Add(new NovelCategory { CategoryId = categoryId });
        }

        _context.Novels.Add(novel);
        await _context.SaveChangesAsync(cancellationToken);
        return novel;
    }

    public async Task UpdateNovelCategoriesAsync(
        Novel novel,
        IEnumerable<int> categoryIds,
        CancellationToken cancellationToken = default)
    {
        var selectedIds = categoryIds.Distinct().ToHashSet();
        var existingItems = novel.NovelCategories.ToList();

        foreach (var item in existingItems.Where(item => !selectedIds.Contains(item.CategoryId)))
        {
            _context.NovelCategories.Remove(item);
        }

        var existingIds = existingItems.Select(item => item.CategoryId).ToHashSet();
        foreach (var categoryId in selectedIds.Where(categoryId => !existingIds.Contains(categoryId)))
        {
            novel.NovelCategories.Add(new NovelCategory
            {
                NovelId = novel.Id,
                CategoryId = categoryId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteNovelAsync(
        Novel novel,
        CancellationToken cancellationToken = default)
    {
        novel.IsActive = false;
        novel.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<Chapter?> GetUserChapterAsync(
        int chapterId,
        int userId,
        bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Chapters
            .Include(chapter => chapter.Novel)
            .Where(chapter => chapter.Id == chapterId
                              && chapter.Novel.AuthorId == userId
                              && chapter.Novel.IsActive);

        if (!asTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<decimal> GetNextChapterNumberAsync(
        int novelId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Chapters
            .Where(chapter => chapter.NovelId == novelId)
            .Where(chapter => chapter.Novel.IsActive)
            .MaxAsync(chapter => (decimal?)chapter.ChapterNumber, cancellationToken)
            ?? 0;
    }

    public async Task<Chapter> CreateChapterAsync(
        Chapter chapter,
        CancellationToken cancellationToken = default)
    {
        _context.Chapters.Add(chapter);
        await _context.SaveChangesAsync(cancellationToken);
        return chapter;
    }

    public async Task DeleteChapterAsync(
        Chapter chapter,
        CancellationToken cancellationToken = default)
    {
        _context.Chapters.Remove(chapter);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
