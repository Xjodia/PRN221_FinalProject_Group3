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
}
