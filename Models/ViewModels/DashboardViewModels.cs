using System.ComponentModel.DataAnnotations;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class DashboardHomeViewModel
{
    public string DisplayName { get; init; } = string.Empty;

    public int NovelCount { get; init; }

    public int ChapterCount { get; init; }
}

public class MyNovelListViewModel
{
    public string? SearchTerm { get; init; }

    public IReadOnlyList<MyNovelListItemViewModel> Novels { get; init; } = [];
}

public class MyNovelListItemViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string PublisherName { get; init; } = string.Empty;

    public string? TranslationGroup { get; init; }

    public int ChapterCount { get; init; }

    public string UpdatedAtText { get; init; } = string.Empty;
}

public class ManagedNovelDetailViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? OriginalAuthor { get; init; }

    public string? Illustrator { get; init; }

    public string StoryType { get; init; } = string.Empty;

    public string? TranslationGroup { get; init; }

    public string Synopsis { get; init; } = string.Empty;

    public string? Note { get; init; }

    public string StatusText { get; init; } = string.Empty;

    public string CoverImage { get; init; } = string.Empty;

    public IReadOnlyList<ManageChapterItemViewModel> Chapters { get; init; } = [];
}

public class ManageChapterItemViewModel
{
    public int Id { get; init; }

    public string Number { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string UpdatedAtText { get; init; } = string.Empty;
}

public class NovelFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
    [StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? OtherNames { get; set; }

    [StringLength(150)]
    public string? OriginalAuthor { get; set; }

    [StringLength(150)]
    public string? Illustrator { get; set; }

    [Required]
    [StringLength(50)]
    public string StoryType { get; set; } = "Truyện dịch";

    [StringLength(150)]
    public string? TranslationGroup { get; set; }

    [StringLength(500)]
    [Url(ErrorMessage = "Ảnh bìa phải là URL hợp lệ.")]
    public string? CoverImage { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tóm tắt.")]
    public string Synopsis { get; set; } = string.Empty;

    public string? Note { get; set; }

    public NovelStatus Status { get; set; } = NovelStatus.Ongoing;

    public List<int> CategoryIds { get; set; } = [];

    public IReadOnlyList<CategoryOptionViewModel> CategoryOptions { get; set; } = [];
}

public class CategoryOptionViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;
}

public class ChapterFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public int NovelId { get; set; }

    public decimal ChapterNumber { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên chương.")]
    [StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập nội dung chương.")]
    public string Content { get; set; } = string.Empty;
}
