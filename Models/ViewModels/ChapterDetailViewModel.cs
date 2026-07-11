using System.ComponentModel.DataAnnotations;

namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class ChapterDetailViewModel
{
    public int NovelId { get; init; }

    public string NovelTitle { get; init; } = string.Empty;

    public int ChapterId { get; init; }

    public string ChapterNumber { get; init; } = string.Empty;

    public string ChapterTitle { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public string UpdatedAtText { get; init; } = string.Empty;

    public string WordCountText { get; init; } = string.Empty;

    public int CommentCount { get; init; }

    public int? PreviousChapterId { get; init; }

    public int? NextChapterId { get; init; }

    public IReadOnlyList<ChapterCommentItemViewModel> Comments { get; init; } = [];
}

public class ChapterCommentItemViewModel
{
    public int Id { get; init; }

    public int ChapterId { get; init; }

    public int UserId { get; init; }

    public string UserName { get; init; } = string.Empty;

    public bool IsUserInactive { get; init; }

    public string Initials { get; init; } = string.Empty;

    public string Level { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public string CreatedAtText { get; init; } = string.Empty;

    public bool IsOwner { get; init; }

    public IReadOnlyList<ChapterCommentItemViewModel> Replies { get; init; } = [];
}

public class ChapterCommentInputViewModel
{
    [Required]
    public int ChapterId { get; set; }

    public int? ParentCommentId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận.")]
    [StringLength(3000, ErrorMessage = "Bình luận không được vượt quá 3000 ký tự.")]
    public string Content { get; set; } = string.Empty;
}
