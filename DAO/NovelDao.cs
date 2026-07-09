using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.Data;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.DAO;

public class NovelDao
{
    private readonly ApplicationDbContext _context;

    public NovelDao(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Novel>> GetFeaturedNovelsAsync(
        int take = 8,
        CancellationToken cancellationToken = default)
    {
        return _context.Novels
            .AsNoTracking()
            .Include(novel => novel.Author)
            .Include(novel => novel.Chapters)
            .OrderByDescending(novel => novel.ViewCount)
            .ThenByDescending(novel => novel.UpdatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Chapter>> GetLatestChaptersAsync(
        int take = 6,
        CancellationToken cancellationToken = default)
    {
        return _context.Chapters
            .AsNoTracking()
            .Include(chapter => chapter.Novel)
                .ThenInclude(novel => novel.NovelCategories)
                    .ThenInclude(item => item.Category)
            .OrderByDescending(chapter => chapter.UpdatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Novel>> GetPopularNovelsAsync(
        int take = 5,
        CancellationToken cancellationToken = default)
    {
        return _context.Novels
            .AsNoTracking()
            .Include(novel => novel.Author)
            .OrderByDescending(novel => novel.ViewCount)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Category>> GetCategoriesAsync(
        int take = 12,
        CancellationToken cancellationToken = default)
    {
        return _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<Novel?> GetDetailAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Novels
            .AsNoTracking()
            .Include(novel => novel.Author)
            .Include(novel => novel.Chapters)
                .ThenInclude(chapter => chapter.Comments)
                    .ThenInclude(comment => comment.User)
            .Include(novel => novel.NovelCategories)
                .ThenInclude(item => item.Category)
            .Include(novel => novel.Followers)
            .FirstOrDefaultAsync(novel => novel.Id == id, cancellationToken);
    }

    public Task<List<Novel>> GetRecommendedNovelsAsync(
        int currentNovelId,
        int take = 3,
        CancellationToken cancellationToken = default)
    {
        return _context.Novels
            .AsNoTracking()
            .Include(novel => novel.Author)
            .Where(novel => novel.Id != currentNovelId)
            .OrderByDescending(novel => novel.ViewCount)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
