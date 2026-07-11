using System.ComponentModel.DataAnnotations;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class SystemHomeViewModel
{
    public int ActiveNovelCount { get; init; }

    public int DeletedNovelCount { get; init; }

    public int UserCount { get; init; }

    public int DeletedUserCount { get; init; }
}

public class SystemNovelListViewModel
{
    public string? SearchTerm { get; init; }

    public bool ShowDeleted { get; init; }

    public IReadOnlyList<SystemNovelItemViewModel> Novels { get; init; } = [];
}

public class SystemNovelItemViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string AuthorName { get; init; } = string.Empty;

    public bool IsAuthorInactive { get; init; }

    public string? TranslationGroup { get; init; }

    public int ChapterCount { get; init; }

    public string UpdatedAtText { get; init; } = string.Empty;
}

public class SystemUserListViewModel
{
    public string? SearchTerm { get; init; }

    public bool ShowDeleted { get; init; }

    public IReadOnlyList<SystemUserItemViewModel> Users { get; init; } = [];
}

public class SystemUserItemViewModel
{
    public int Id { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public bool IsInactive { get; init; }

    public string Username { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public int NovelCount { get; init; }

    public string UpdatedAtText { get; init; } = string.Empty;
}

public class SystemUserDetailViewModel
{
    public SystemUserEditViewModel User { get; set; } = new();

    public SystemPasswordResetViewModel Password { get; set; } = new();
}

public class SystemUserEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 50 ký tự.")]
    [RegularExpression(
        @"^[a-zA-Z0-9._]+$",
        ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, chữ số, dấu chấm và dấu gạch dưới.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên hiển thị.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên hiển thị từ 2 đến 100 ký tự.")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Lời giới thiệu tối đa 500 ký tự.")]
    public string? Bio { get; set; }

    [StringLength(500)]
    [Url(ErrorMessage = "Avatar phải là URL hợp lệ.")]
    public string? AvatarUrl { get; set; }

    public UserRole Role { get; set; }

    public UserStatus Status { get; set; }

    public string CreatedAtText { get; set; } = string.Empty;

    public string UpdatedAtText { get; set; } = string.Empty;

    public int NovelCount { get; set; }

    public int ChapterCount { get; set; }

    public int CommentCount { get; set; }
}

public class SystemPasswordResetViewModel
{
    [Required]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
