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
            .Where(novel => novel.IsActive)
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
            .Where(chapter => chapter.Novel.IsActive)
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
            .Where(novel => novel.IsActive)
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
            .Include(novel => novel.Comments)
                .ThenInclude(comment => comment.User)
            .Include(novel => novel.NovelCategories)
                .ThenInclude(item => item.Category)
            .Include(novel => novel.Followers)
            .FirstOrDefaultAsync(novel => novel.Id == id && novel.IsActive, cancellationToken);
    }

    public Task<List<Novel>> GetRecommendedNovelsAsync(
        int currentNovelId,
        int take = 3,
        CancellationToken cancellationToken = default)
    {
        return _context.Novels
            .AsNoTracking()
            .Include(novel => novel.Author)
            .Where(novel => novel.Id != currentNovelId && novel.IsActive)
            .OrderByDescending(novel => novel.ViewCount)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> NovelExistsAsync(
        int novelId,
        CancellationToken cancellationToken = default)
    {
        return _context.Novels.AnyAsync(
            novel => novel.Id == novelId && novel.IsActive,
            cancellationToken);
    }

    public Task<bool> CommentBelongsToNovelAsync(
        int commentId,
        int novelId,
        CancellationToken cancellationToken = default)
    {
        return _context.NovelComments.AnyAsync(
            comment => comment.Id == commentId
                       && comment.NovelId == novelId
                       && comment.Novel.IsActive,
            cancellationToken);
    }

    public async Task<NovelComment> CreateCommentAsync(
        NovelComment comment,
        CancellationToken cancellationToken = default)
    {
        _context.NovelComments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);
        return comment;
    }

    public Task<List<Novel>> SearchNovelsAsync(
        string keyword,
        NovelStatus? status = null,
        string sort = "az",
        int take = 0,
        CancellationToken cancellationToken = default)
    {
        var normalizedKeyword = keyword.Trim().ToLower();
        var query = _context.Novels
            .AsNoTracking()
            .Include(novel => novel.Author)
            .Include(novel => novel.Chapters)
            .Include(novel => novel.NovelCategories)
                .ThenInclude(item => item.Category)
            .Where(novel => novel.IsActive);

        if (!string.IsNullOrWhiteSpace(normalizedKeyword))
        {
            query = query.Where(novel =>
                novel.Title.ToLower().Contains(normalizedKeyword)
                || (novel.OtherNames != null && novel.OtherNames.ToLower().Contains(normalizedKeyword))
                || (novel.OriginalAuthor != null && novel.OriginalAuthor.ToLower().Contains(normalizedKeyword))
                || novel.Author.DisplayName.ToLower().Contains(normalizedKeyword)
                || novel.Author.Username.ToLower().Contains(normalizedKeyword)
                || novel.NovelCategories.Any(item => item.Category.Name.ToLower().Contains(normalizedKeyword))
                || novel.Synopsis.ToLower().Contains(normalizedKeyword));
        }

        if (status.HasValue)
        {
            query = query.Where(novel => novel.Status == status.Value);
        }

        query = sort switch
        {
            "az" => query
                .OrderBy(novel => EF.Functions.Like(novel.Title, "[0-9]%") ? 0 : 1)
                .ThenBy(novel => novel.Title),
            "latest" => query.OrderByDescending(novel => novel.UpdatedAt),
            "popular" => query.OrderByDescending(novel => novel.ViewCount)
                .ThenByDescending(novel => novel.UpdatedAt),
            _ => query
                .OrderByDescending(novel => novel.Title.ToLower().Contains(normalizedKeyword))
                .ThenBy(novel => EF.Functions.Like(novel.Title, "[0-9]%") ? 0 : 1)
                .ThenBy(novel => novel.Title)
                .ThenByDescending(novel => novel.ViewCount)
                .ThenByDescending(novel => novel.UpdatedAt)
        };

        if (take > 0)
        {
            query = query.Take(take);
        }

        return query.ToListAsync(cancellationToken);
    }

    public Task<List<User>> SearchMembersAsync(
        string keyword,
        int take = 12,
        CancellationToken cancellationToken = default)
    {
        var normalizedKeyword = keyword.Trim().ToLower();
        return _context.Users
            .AsNoTracking()
            .Include(user => user.AuthoredNovels)
            .Where(user =>
                user.DisplayName.ToLower().Contains(normalizedKeyword)
                || user.Username.ToLower().Contains(normalizedKeyword)
                || user.Email.ToLower().Contains(normalizedKeyword)
                || (user.Bio != null && user.Bio.ToLower().Contains(normalizedKeyword)))
            .OrderByDescending(user => user.AuthoredNovels.Count)
            .ThenBy(user => user.DisplayName)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
