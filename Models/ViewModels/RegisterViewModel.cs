using System.ComponentModel.DataAnnotations;

namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class RegisterViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [StringLength(
        50,
        MinimumLength = 3,
        ErrorMessage = "Tên đăng nhập phải có từ 3 đến 50 ký tự.")]
    [RegularExpression(
        @"^[a-zA-Z0-9._]+$",
        ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, chữ số, dấu chấm và dấu gạch dưới.")]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên hiển thị.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Tên hiển thị phải có từ 2 đến 100 ký tự.")]
    [Display(Name = "Tên hiển thị")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "Mật khẩu phải có từ 8 đến 100 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Đồng ý với điều khoản sử dụng")]
    public bool AcceptTerms { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!AcceptTerms)
        {
            yield return new ValidationResult(
                "Bạn cần đồng ý với điều khoản sử dụng.",
                [nameof(AcceptTerms)]);
        }
    }
}
