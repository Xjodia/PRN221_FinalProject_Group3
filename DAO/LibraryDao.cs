using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.Data;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.DAO;

public class LibraryDao
{
    private readonly ApplicationDbContext _context;

    public LibraryDao(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> IsFollowingAsync(
        int userId,
        int novelId,
        CancellationToken cancellationToken = default)
    {
        return _context.Follows.AnyAsync(
            follow => follow.UserId == userId
                      && follow.NovelId == novelId
                      && follow.Novel.IsActive,
            cancellationToken);
    }

    public Task<Follow?> GetFollowAsync(
        int userId,
        int novelId,
        bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Follows
            .Where(follow => follow.UserId == userId && follow.NovelId == novelId);

        if (!asTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ActiveNovelExistsAsync(
        int novelId,
        CancellationToken cancellationToken = default)
    {
        return _context.Novels.AnyAsync(
            novel => novel.Id == novelId && novel.IsActive,
            cancellationToken);
    }

    public async Task AddFollowAsync(
        Follow follow,
        CancellationToken cancellationToken = default)
    {
        _context.Follows.Add(follow);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFollowAsync(
        Follow follow,
        CancellationToken cancellationToken = default)
    {
        _context.Follows.Remove(follow);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Follow>> GetFollowedNovelsAsync(
        int userId,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Follows
            .AsNoTracking()
            .Include(follow => follow.Novel)
                .ThenInclude(novel => novel.Author)
            .Include(follow => follow.Novel)
                .ThenInclude(novel => novel.Chapters)
            .Where(follow => follow.UserId == userId && follow.Novel.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim().ToLower();
            query = query.Where(follow =>
                follow.Novel.Title.ToLower().Contains(keyword)
                || follow.Novel.Author.DisplayName.ToLower().Contains(keyword));
        }

        return query
            .OrderByDescending(follow => follow.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ReadingHistory>> GetReadingHistoriesAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return _context.ReadingHistories
            .AsNoTracking()
            .Include(history => history.Novel)
            .Include(history => history.Chapter)
            .Where(history => history.UserId == userId && history.Novel.IsActive)
            .OrderByDescending(history => history.LastReadAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Chapter?> GetActiveChapterAsync(
        int chapterId,
        CancellationToken cancellationToken = default)
    {
        return _context.Chapters
            .AsNoTracking()
            .FirstOrDefaultAsync(
                chapter => chapter.Id == chapterId && chapter.Novel.IsActive,
                cancellationToken);
    }

    public Task<ReadingHistory?> GetReadingHistoryAsync(
        int userId,
        int novelId,
        bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ReadingHistories
            .Where(history => history.UserId == userId && history.NovelId == novelId);

        if (!asTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveReadingHistoryAsync(
        ReadingHistory history,
        bool isNew,
        CancellationToken cancellationToken = default)
    {
        if (isNew)
        {
            _context.ReadingHistories.Add(history);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
