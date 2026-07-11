using System.ComponentModel.DataAnnotations;

namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class AccountSettingsViewModel
{
    public ProfileEditViewModel Profile { get; set; } = new();

    public ChangePasswordViewModel Password { get; set; } = new();
}

public class ProfileEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên hiển thị.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên hiển thị từ 2 đến 100 ký tự.")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Lời giới thiệu tối đa 500 ký tự.")]
    public string? Bio { get; set; }

    [StringLength(500)]
    [Url(ErrorMessage = "Avatar phải là URL hợp lệ.")]
    public string? AvatarUrl { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}

public class ChangePasswordViewModel
{
    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
