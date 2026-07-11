using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.DAO;
using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Interface;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Implement;

public class UserService : IUserService
{
    private readonly UserDao _userDao;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserDao userDao,
        ILogger<UserService> logger)
    {
        _userDao = userDao;
        _logger = logger;
    }

    public async Task<RegisterResult> RegisterUserAsync(
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
            return RegisterResult.Failure(errors);
        }

        var now = DateTimeOffset.UtcNow;
        var user = new User
        {
            Username = username,
            Email = email,
            DisplayName = model.DisplayName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = UserRole.Reader,
            Status = UserStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        try
        {
            await _userDao.CreateAsync(user, cancellationToken);
            return RegisterResult.Success();
        }
        catch (DbUpdateException exception)
        {
            _logger.LogWarning(
                exception,
                "Không thể tạo tài khoản {Username}.",
                username);

            return RegisterResult.Failure(
                string.Empty,
                "Không thể tạo tài khoản. Tên đăng nhập hoặc email có thể đã được sử dụng.");
        }
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
                "Tài khoản hiện không thể đăng nhập.");
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
}
