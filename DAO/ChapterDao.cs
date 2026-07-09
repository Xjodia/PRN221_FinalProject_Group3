using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.Data;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.DAO;

public class ChapterDao
{
    private readonly ApplicationDbContext _context;

    public ChapterDao(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Chapter?> GetDetailAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Chapters
            .AsNoTracking()
            .Include(chapter => chapter.Novel)
                .ThenInclude(novel => novel.Author)
            .Include(chapter => chapter.Comments)
                .ThenInclude(comment => comment.User)
            .FirstOrDefaultAsync(chapter => chapter.Id == id, cancellationToken);
    }

    public Task<Chapter?> GetPreviousChapterAsync(
        int novelId,
        decimal currentChapterNumber,
        CancellationToken cancellationToken = default)
    {
        return _context.Chapters
            .AsNoTracking()
            .Where(chapter => chapter.NovelId == novelId
                              && chapter.ChapterNumber < currentChapterNumber)
            .OrderByDescending(chapter => chapter.ChapterNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Chapter?> GetNextChapterAsync(
        int novelId,
        decimal currentChapterNumber,
        CancellationToken cancellationToken = default)
    {
        return _context.Chapters
            .AsNoTracking()
            .Where(chapter => chapter.NovelId == novelId
                              && chapter.ChapterNumber > currentChapterNumber)
            .OrderBy(chapter => chapter.ChapterNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ChapterExistsAsync(
        int chapterId,
        CancellationToken cancellationToken = default)
    {
        return _context.Chapters.AnyAsync(
            chapter => chapter.Id == chapterId,
            cancellationToken);
    }

    public Task<bool> CommentBelongsToChapterAsync(
        int commentId,
        int chapterId,
        CancellationToken cancellationToken = default)
    {
        return _context.ChapterComments.AnyAsync(
            comment => comment.Id == commentId && comment.ChapterId == chapterId,
            cancellationToken);
    }

    public async Task<ChapterComment> CreateCommentAsync(
        ChapterComment comment,
        CancellationToken cancellationToken = default)
    {
        _context.ChapterComments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);
        return comment;
    }
}
