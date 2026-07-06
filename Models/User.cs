using System.ComponentModel.DataAnnotations;

namespace PRN221_FinalProject_Group3.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(
        @"^[a-zA-Z0-9._]+$",
        ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, chữ số, dấu chấm và dấu gạch dưới.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500)]
    [Url]
    public string? AvatarUrl { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    public UserRole Role { get; set; } = UserRole.Reader;

    public UserStatus Status { get; set; } = UserStatus.Active;

    public bool IsEmailVerified { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Novel> AuthoredNovels { get; set; } = new List<Novel>();

    public ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();

    public ICollection<Follow> FollowedNovels { get; set; } = new List<Follow>();

    public ICollection<ChapterComment> ChapterComments { get; set; } = new List<ChapterComment>();
}

public enum UserRole
{
    Reader = 0,
    Author = 1,
    Moderator = 2,
    Admin = 3
}

public enum UserStatus
{
    Inactive = 0,
    Active = 1,
    Suspended = 2,
    Banned = 3
}
