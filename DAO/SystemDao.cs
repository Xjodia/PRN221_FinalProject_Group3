using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.Data;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.DAO;

public class SystemDao
{
    private readonly ApplicationDbContext _context;

    public SystemDao(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> CountNovelsAsync(
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        return _context.Novels
            .AsNoTracking()
            .Where(novel => novel.IsActive == isActive)
            .CountAsync(cancellationToken);
    }

    public Task<int> CountUsersAsync(CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }

    public Task<List<Novel>> GetNovelsAsync(
        bool isActive,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Novels
            .AsNoTracking()
            .Include(novel => novel.Author)
            .Include(novel => novel.Chapters)
            .Where(novel => novel.IsActive == isActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim().ToLower();
            query = query.Where(novel =>
                novel.Title.ToLower().Contains(keyword)
                || novel.Author.DisplayName.ToLower().Contains(keyword)
                || novel.Author.Username.ToLower().Contains(keyword)
                || (novel.TranslationGroup != null && novel.TranslationGroup.ToLower().Contains(keyword)));
        }

        return query
            .OrderByDescending(novel => novel.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Novel?> GetNovelAsync(
        int novelId,
        bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Novels
            .Where(novel => novel.Id == novelId);

        if (!asTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
