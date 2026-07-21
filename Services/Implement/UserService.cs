using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.DAO;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Email;
using PRN221_FinalProject_Group3.Services.Interface;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Implement;

public class UserService : IUserService
{
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromMinutes(5);
    private const int MaximumCodeAttempts = 5;

    private readonly UserDao _userDao;
    private readonly EmailVerificationDao _verificationDao;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserDao userDao,
        EmailVerificationDao verificationDao,
        IEmailSender emailSender,
        ILogger<UserService> logger)
    {
        _userDao = userDao;
        _verificationDao = verificationDao;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<AccountVerificationResult> StartRegistrationAsync(
        RegisterViewModel model,
        CancellationToken cancellationToken = default)
    {
        var username = model.Username.Trim();
        var email = model.Email.Trim().ToLowerInvariant();
        var errors = new Dictionary<string, string>();

        if (await _userDao.UsernameExistsAsync(username, cancellationToken))
        {
            errors[nameof(model.Username)] = "Tên đăng nhập đã được sử dụng.";
        }

        if (await _userDao.EmailExistsAsync(email, cancellationToken))
        {
            errors[nameof(model.Email)] = "Email đã được sử dụng.";
        }

        if (errors.Count > 0)
        {
            return AccountVerificationResult.Failure(errors);
        }

        var now = DateTimeOffset.UtcNow;
        var code = GenerateCode();
        var verification = new EmailVerification
        {
            Email = email,
            Purpose = EmailVerificationPurpose.Registration,
            CodeHash = BCrypt.Net.BCrypt.HashPassword(code),
            ExpiresAt = now.Add(CodeLifetime),
            Username = username,
            DisplayName = model.DisplayName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            CreatedAt = now
        };

        try
        {
            await _verificationDao.DeleteExpiredAsync(cancellationToken);
            await _verificationDao.DeletePendingAsync(
                email,
                EmailVerificationPurpose.Registration,
                cancellationToken);
            await _verificationDao.CreateAsync(verification, cancellationToken);
            await _emailSender.SendVerificationCodeAsync(
                email,
                code,
                "đăng ký tài khoản",
                cancellationToken);

            return AccountVerificationResult.Success(
                verification.Id,
                verification.Email,
                verification.ExpiresAt);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(
                exception,
                "Không thể gửi mã xác thực đăng ký cho {Email}.",
                email);

            await RemoveVerificationSafelyAsync(verification, cancellationToken);
            return AccountVerificationResult.Failure(
                string.Empty,
                "Không thể gửi mã xác thực. Vui lòng kiểm tra cấu hình email và thử lại.");
        }
    }

    public async Task<AccountVerificationResult> GetVerificationAsync(
        Guid flowId,
        EmailVerificationPurpose purpose,
        CancellationToken cancellationToken = default)
    {
        var verification = await _verificationDao.GetAsync(
            flowId,
            purpose,
            cancellationToken: cancellationToken);

        if (verification is null)
        {
            return AccountVerificationResult.Failure(
                string.Empty,
                "Yêu cầu xác thực không tồn tại hoặc đã bị hủy.");
        }

        if (verification.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            await DeleteVerificationAsync(verification, cancellationToken);
            return AccountVerificationResult.Failure(
                string.Empty,
                "Mã xác thực đã hết hạn. Vui lòng thực hiện lại yêu cầu.",
                isExpired: true);
        }

        if (verification.VerifiedAt is not null)
        {
            return AccountVerificationResult.Failure(
                string.Empty,
                "Mã xác thực đã được sử dụng.");
        }

        return AccountVerificationResult.Success(
            verification.Id,
            verification.Email,
            verification.ExpiresAt);
    }

    public async Task<AccountVerificationResult> CompleteRegistrationAsync(
        Guid flowId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var verification = await GetActiveVerificationAsync(
            flowId,
            EmailVerificationPurpose.Registration,
            cancellationToken);

        var codeError = await ValidateCodeAsync(verification, code, cancellationToken);
        if (codeError is not null)
        {
            return codeError;
        }

        if (verification is null
            || string.IsNullOrWhiteSpace(verification.Username)
            || string.IsNullOrWhiteSpace(verification.DisplayName)
            || string.IsNullOrWhiteSpace(verification.PasswordHash))
        {
            return AccountVerificationResult.Failure(
                string.Empty,
                "Dữ liệu đăng ký không hợp lệ. Vui lòng đăng ký lại.");
        }

        var errors = new Dictionary<string, string>();
        if (await _userDao.UsernameExistsAsync(verification.Username, cancellationToken))
        {
            errors[string.Empty] = "Tên đăng nhập đã được sử dụng. Vui lòng đăng ký lại.";
        }

        if (await _userDao.EmailExistsAsync(verification.Email, cancellationToken))
        {
            errors[string.Empty] = "Email đã được sử dụng. Vui lòng đăng nhập hoặc dùng email khác.";
        }

        if (errors.Count > 0)
        {
            await DeleteVerificationAsync(verification, cancellationToken);
            return AccountVerificationResult.Failure(errors);
        }

        var now = DateTimeOffset.UtcNow;
        var user = new User
        {
            Username = verification.Username,
            Email = verification.Email,
            DisplayName = verification.DisplayName,
            PasswordHash = verification.PasswordHash,
            Role = UserRole.Reader,
            Status = UserStatus.Active,
            IsEmailVerified = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        try
        {
            _userDao.Add(user);
            _verificationDao.Remove(verification);
            await _userDao.SaveChangesAsync(cancellationToken);

            return AccountVerificationResult.Success(flowId);
        }
        catch (DbUpdateException exception)
        {
            _logger.LogWarning(
                exception,
                "Không thể hoàn tất đăng ký cho {Email}.",
                verification.Email);

            return AccountVerificationResult.Failure(
                string.Empty,
                "Không thể tạo tài khoản. Tên đăng nhập hoặc email có thể đã được sử dụng.");
        }
    }

    public async Task<AccountVerificationResult> StartPasswordResetAsync(
        ForgotPasswordViewModel model,
        CancellationToken cancellationToken = default)
    {
        var email = model.Email.Trim().ToLowerInvariant();
        var user = await _userDao.FindByEmailAsync(
            email,
            cancellationToken: cancellationToken);

        if (user is null)
        {
            return AccountVerificationResult.Failure(
                nameof(model.Email),
                "Không tìm thấy tài khoản sử dụng email này.");
        }

        var now = DateTimeOffset.UtcNow;
        var code = GenerateCode();
        var verification = new EmailVerification
        {
            Email = email,
            Purpose = EmailVerificationPurpose.PasswordReset,
            CodeHash = BCrypt.Net.BCrypt.HashPassword(code),
            ExpiresAt = now.Add(CodeLifetime),
            UserId = user.Id,
            CreatedAt = now
        };

        try
        {
            await _verificationDao.DeleteExpiredAsync(cancellationToken);
            await _verificationDao.DeletePendingAsync(
                email,
                EmailVerificationPurpose.PasswordReset,
                cancellationToken);
            await _verificationDao.CreateAsync(verification, cancellationToken);
            await _emailSender.SendVerificationCodeAsync(
                email,
                code,
                "đặt lại mật khẩu",
                cancellationToken);

            return AccountVerificationResult.Success(
                verification.Id,
                verification.Email,
                verification.ExpiresAt);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(
                exception,
                "Không thể gửi mã đặt lại mật khẩu cho {Email}.",
                email);

            await RemoveVerificationSafelyAsync(verification, cancellationToken);
            return AccountVerificationResult.Failure(
                string.Empty,
                "Không thể gửi mã xác thực. Vui lòng thử lại sau.");
        }
    }

    public async Task<AccountVerificationResult> VerifyPasswordResetCodeAsync(
        Guid flowId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var verification = await GetActiveVerificationAsync(
            flowId,
            EmailVerificationPurpose.PasswordReset,
            cancellationToken);

        var codeError = await ValidateCodeAsync(verification, code, cancellationToken);
        if (codeError is not null)
        {
            return codeError;
        }

        if (verification is null || verification.UserId is null)
        {
            return AccountVerificationResult.Failure(
                string.Empty,
                "Yêu cầu đặt lại mật khẩu không hợp lệ.");
        }

        var now = DateTimeOffset.UtcNow;
        var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        verification.VerifiedAt = now;
        verification.ResetTokenHash = HashToken(resetToken);
        verification.ResetTokenExpiresAt = now.Add(ResetTokenLifetime);
        await _verificationDao.SaveChangesAsync(cancellationToken);

        return AccountVerificationResult.Success(
            flowId,
            verification.Email,
            verification.ResetTokenExpiresAt,
            resetToken);
    }

    public async Task<bool> IsResetTokenValidAsync(
        Guid flowId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var verification = await _verificationDao.GetAsync(
            flowId,
            EmailVerificationPurpose.PasswordReset,
            cancellationToken: cancellationToken);

        return IsResetTokenValid(verification, token);
    }

    public async Task<RegisterResult> ResetPasswordAsync(
        ResetPasswordViewModel model,
        CancellationToken cancellationToken = default)
    {
        var verification = await _verificationDao.GetAsync(
            model.FlowId,
            EmailVerificationPurpose.PasswordReset,
            asTracking: true,
            cancellationToken);

        if (!IsResetTokenValid(verification, model.Token)
            || verification?.UserId is null)
        {
            return RegisterResult.Failure(
                string.Empty,
                "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");
        }

        var user = await _userDao.GetByIdAsync(
            verification.UserId.Value,
            asTracking: true,
            cancellationToken);

        if (user is null)
        {
            _verificationDao.Remove(verification);
            await _verificationDao.SaveChangesAsync(cancellationToken);
            return RegisterResult.Failure(string.Empty, "Không tìm thấy tài khoản.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;
        _verificationDao.Remove(verification);
        await _userDao.SaveChangesAsync(cancellationToken);

        return RegisterResult.Success();
    }

    public async Task<LoginResult> LoginAsync(
        LoginViewModel model,
        CancellationToken cancellationToken = default)
    {
        var user = await _userDao.FindByLoginAsync(
            model.Identifier,
            cancellationToken);

        if (user is null || !IsPasswordValid(model.Password, user.PasswordHash))
        {
            return LoginResult.Failure(
                "Tên đăng nhập, email hoặc mật khẩu không chính xác.");
        }

        if (user.Status != UserStatus.Active)
        {
            return LoginResult.Failure(
                user.Status == UserStatus.Inactive
                    ? "Tài khoản của bạn đã bị hệ thống ngưng hoạt động."
                    : "Tài khoản hiện không thể đăng nhập.");
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await _userDao.SaveChangesAsync(cancellationToken);

        return LoginResult.Success(user);
    }

    public async Task<AccountSettingsViewModel?> GetAccountSettingsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userDao.GetByIdAsync(userId, cancellationToken: cancellationToken);
        if (user is null)
        {
            return null;
        }

        return new AccountSettingsViewModel
        {
            Profile = new ProfileEditViewModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                Username = user.Username,
                Email = user.Email
            },
            Password = new ChangePasswordViewModel()
        };
    }

    public async Task<RegisterResult> UpdateProfileAsync(
        ProfileEditViewModel model,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (model.Id != userId)
        {
            return RegisterResult.Failure(string.Empty, "Bạn không có quyền cập nhật hồ sơ này.");
        }

        var user = await _userDao.GetByIdAsync(
            userId,
            asTracking: true,
            cancellationToken);

        if (user is null)
        {
            return RegisterResult.Failure(string.Empty, "Không tìm thấy tài khoản.");
        }

        user.DisplayName = model.DisplayName.Trim();
        user.Bio = string.IsNullOrWhiteSpace(model.Bio) ? null : model.Bio.Trim();
        user.AvatarUrl = string.IsNullOrWhiteSpace(model.AvatarUrl) ? null : model.AvatarUrl.Trim();
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _userDao.SaveChangesAsync(cancellationToken);
        return RegisterResult.Success();
    }

    public async Task<RegisterResult> ChangePasswordAsync(
        ChangePasswordViewModel model,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userDao.GetByIdAsync(
            userId,
            asTracking: true,
            cancellationToken);

        if (user is null)
        {
            return RegisterResult.Failure(string.Empty, "Không tìm thấy tài khoản.");
        }

        if (HasExistingPassword(user.PasswordHash)
            && !IsPasswordValid(model.CurrentPassword ?? string.Empty, user.PasswordHash))
        {
            return RegisterResult.Failure(
                nameof(model.CurrentPassword),
                "Mật khẩu hiện tại không chính xác.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _userDao.SaveChangesAsync(cancellationToken);
        return RegisterResult.Success();
    }

    private async Task<EmailVerification?> GetActiveVerificationAsync(
        Guid flowId,
        EmailVerificationPurpose purpose,
        CancellationToken cancellationToken)
    {
        return await _verificationDao.GetAsync(
            flowId,
            purpose,
            asTracking: true,
            cancellationToken);
    }

    private async Task<AccountVerificationResult?> ValidateCodeAsync(
        EmailVerification? verification,
        string code,
        CancellationToken cancellationToken)
    {
        if (verification is null)
        {
            return AccountVerificationResult.Failure(
                string.Empty,
                "Yêu cầu xác thực không tồn tại hoặc đã bị hủy.");
        }

        if (verification.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            await DeleteVerificationAsync(verification, cancellationToken);
            return AccountVerificationResult.Failure(
                string.Empty,
                "Mã xác thực đã hết hạn sau 5 phút. Vui lòng thực hiện lại yêu cầu.",
                isExpired: true);
        }

        if (verification.VerifiedAt is not null)
        {
            return AccountVerificationResult.Failure(
                string.Empty,
                "Mã xác thực đã được sử dụng.");
        }

        if (!BCrypt.Net.BCrypt.Verify(code, verification.CodeHash))
        {
            verification.FailedAttempts++;
            if (verification.FailedAttempts >= MaximumCodeAttempts)
            {
                _verificationDao.Remove(verification);
                await _verificationDao.SaveChangesAsync(cancellationToken);
                return AccountVerificationResult.Failure(
                    nameof(VerifyEmailCodeViewModel.Code),
                    "Bạn đã nhập sai quá 5 lần. Yêu cầu xác thực đã bị hủy.");
            }

            await _verificationDao.SaveChangesAsync(cancellationToken);
            return AccountVerificationResult.Failure(
                nameof(VerifyEmailCodeViewModel.Code),
                $"Mã xác thực không đúng. Bạn còn {MaximumCodeAttempts - verification.FailedAttempts} lần thử.");
        }

        return null;
    }

    private bool IsPasswordValid(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (BCrypt.Net.SaltParseException exception)
        {
            _logger.LogWarning(exception, "Password hash không hợp lệ.");
            return false;
        }
    }

    private static bool HasExistingPassword(string? passwordHash)
    {
        return !string.IsNullOrWhiteSpace(passwordHash)
               && !string.Equals(
                   passwordHash,
                   "seeded-account-not-for-login",
                   StringComparison.Ordinal);
    }

    private static string GenerateCode()
    {
        return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }

    private static bool IsResetTokenValid(
        EmailVerification? verification,
        string token)
    {
        if (verification is null
            || verification.VerifiedAt is null
            || verification.ResetTokenExpiresAt <= DateTimeOffset.UtcNow
            || string.IsNullOrWhiteSpace(verification.ResetTokenHash)
            || string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        try
        {
            var expectedHash = Convert.FromHexString(verification.ResetTokenHash);
            var actualHash = Convert.FromHexString(HashToken(token));
            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private async Task DeleteVerificationAsync(
        EmailVerification verification,
        CancellationToken cancellationToken)
    {
        _verificationDao.Remove(verification);
        await _verificationDao.SaveChangesAsync(cancellationToken);
    }

    private async Task RemoveVerificationSafelyAsync(
        EmailVerification verification,
        CancellationToken cancellationToken)
    {
        try
        {
            _verificationDao.Remove(verification);
            await _verificationDao.SaveChangesAsync(cancellationToken);
        }
        catch (Exception cleanupException)
        {
            _logger.LogWarning(
                cleanupException,
                "Không thể dọn yêu cầu xác thực {VerificationId}.",
                verification.Id);
        }
    }
}
