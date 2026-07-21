using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Interface;

public interface INovelService
{
    Task<HomeViewModel> GetHomePageAsync(CancellationToken cancellationToken = default);

    Task<NovelDetailViewModel?> GetNovelDetailAsync(
        int id,
        int? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task<SearchViewModel> SearchAsync(
        string? query,
        string? tab = null,
        string? status = null,
        string? sort = null,
        string? author = null,
        IReadOnlyCollection<int>? categoryIds = null,
        CancellationToken cancellationToken = default);

    Task<CommentResult> AddCommentAsync(
        NovelCommentInputViewModel model,
        int userId,
        CancellationToken cancellationToken = default);
}
