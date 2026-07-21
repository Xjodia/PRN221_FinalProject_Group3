using System.ComponentModel.DataAnnotations;

namespace PRN221_FinalProject_Group3.Models;

public class EmailVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    public EmailVerificationPurpose Purpose { get; set; }

    [Required]
    [StringLength(100)]
    public string CodeHash { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }

    public int FailedAttempts { get; set; }

    public DateTimeOffset? VerifiedAt { get; set; }

    [StringLength(64)]
    public string? ResetTokenHash { get; set; }

    public DateTimeOffset? ResetTokenExpiresAt { get; set; }

    public int? UserId { get; set; }

    public User? User { get; set; }

    [StringLength(50)]
    public string? Username { get; set; }

    [StringLength(100)]
    public string? DisplayName { get; set; }

    [StringLength(255)]
    public string? PasswordHash { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public enum EmailVerificationPurpose
{
    Registration = 0,
    PasswordReset = 1
}
