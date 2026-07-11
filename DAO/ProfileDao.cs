using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.Data;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.DAO;

public class ProfileDao
{
    private readonly ApplicationDbContext _context;

    public ProfileDao(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetPublicProfileAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AsNoTracking()
            .Include(user => user.AuthoredNovels)
                .ThenInclude(novel => novel.Chapters)
            .FirstOrDefaultAsync(
                user => user.Id == userId && user.Status == UserStatus.Active,
                cancellationToken);
    }

    public Task<int> CountAuthoredChaptersAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Chapters
            .AsNoTracking()
            .Where(chapter => chapter.Novel.AuthorId == userId && chapter.Novel.IsActive)
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountCommentsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var novelCommentCount = await _context.NovelComments
            .AsNoTracking()
            .Where(comment => comment.UserId == userId)
            .CountAsync(cancellationToken);

        var chapterCommentCount = await _context.ChapterComments
            .AsNoTracking()
            .Where(comment => comment.UserId == userId)
            .CountAsync(cancellationToken);

        return novelCommentCount + chapterCommentCount;
    }
}
