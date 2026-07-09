using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Interface;

public interface IChapterService
{
    Task<ChapterDetailViewModel?> GetChapterDetailAsync(
        int chapterId,
        CancellationToken cancellationToken = default);

    Task<CommentResult> AddCommentAsync(
        ChapterCommentInputViewModel model,
        int userId,
        CancellationToken cancellationToken = default);
}
