using System.ComponentModel.DataAnnotations;

namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class VerifyEmailCodeViewModel
{
    [Required]
    public Guid FlowId { get; set; }

    public string Email { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã xác thực.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Mã xác thực phải gồm đúng 6 chữ số.")]
    [Display(Name = "Mã xác thực")]
    public string Code { get; set; } = string.Empty;
}

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordViewModel
{
    [Required]
    public Guid FlowId { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "Mật khẩu phải có từ 8 đến 100 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu nhập lại không khớp.")]
    [Display(Name = "Nhập lại mật khẩu mới")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
