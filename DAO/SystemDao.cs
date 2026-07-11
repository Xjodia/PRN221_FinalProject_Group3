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

    public Task<int> CountUsersAsync(
        UserStatus status,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AsNoTracking()
            .Where(user => user.Status == status)
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

    public Task<List<User>> GetUsersAsync(
        bool showDeleted,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users
            .AsNoTracking()
            .Include(user => user.AuthoredNovels)
            .Where(user => showDeleted
                ? user.Status == UserStatus.Inactive
                : user.Status != UserStatus.Inactive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim().ToLower();
            query = query.Where(user =>
                user.DisplayName.ToLower().Contains(keyword)
                || user.Username.ToLower().Contains(keyword)
                || user.Email.ToLower().Contains(keyword)
                || (user.Bio != null && user.Bio.ToLower().Contains(keyword)));
        }

        return query
            .OrderByDescending(user => user.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<User?> GetUserAsync(
        int userId,
        bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users
            .Include(user => user.AuthoredNovels)
                .ThenInclude(novel => novel.Chapters)
            .Include(user => user.ChapterComments)
            .Include(user => user.NovelComments)
            .Where(user => user.Id == userId);

        if (!asTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> UsernameExistsForOtherUserAsync(
        int userId,
        string username,
        CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim().ToLower();
        return _context.Users.AnyAsync(
            user => user.Id != userId && user.Username.ToLower() == normalizedUsername,
            cancellationToken);
    }

    public Task<bool> EmailExistsForOtherUserAsync(
        int userId,
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLower();
        return _context.Users.AnyAsync(
            user => user.Id != userId && user.Email.ToLower() == normalizedEmail,
            cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
