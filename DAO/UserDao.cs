using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.Data;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.DAO;

public class UserDao
{
    private readonly ApplicationDbContext _context;

    public UserDao(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> UsernameExistsAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim().ToLower();

        return _context.Users.AnyAsync(
            user => user.Username.ToLower() == normalizedUsername,
            cancellationToken);
    }

    public Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLower();

        return _context.Users.AnyAsync(
            user => user.Email.ToLower() == normalizedEmail,
            cancellationToken);
    }

    public async Task<User> CreateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public Task<User?> FindByLoginAsync(
        string identifier,
        CancellationToken cancellationToken = default)
    {
        var normalizedIdentifier = identifier.Trim().ToLower();

        return _context.Users.FirstOrDefaultAsync(
            user => user.Username.ToLower() == normalizedIdentifier
                    || user.Email.ToLower() == normalizedIdentifier,
            cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
