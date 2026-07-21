using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.Data;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.DAO;

public class EmailVerificationDao
{
    private readonly ApplicationDbContext _context;

    public EmailVerificationDao(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(
        EmailVerification verification,
        CancellationToken cancellationToken = default)
    {
        _context.EmailVerifications.Add(verification);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<EmailVerification?> GetAsync(
        Guid id,
        EmailVerificationPurpose purpose,
        bool asTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.EmailVerifications
            .Where(item => item.Id == id && item.Purpose == purpose);

        if (!asTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task DeletePendingAsync(
        string email,
        EmailVerificationPurpose purpose,
        CancellationToken cancellationToken = default)
    {
        await _context.EmailVerifications
            .Where(item => item.Email == email && item.Purpose == purpose)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteExpiredAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        await _context.EmailVerifications
            .Where(item =>
                (item.VerifiedAt == null && item.ExpiresAt <= now)
                || (item.VerifiedAt != null
                    && item.ResetTokenExpiresAt != null
                    && item.ResetTokenExpiresAt <= now))
            .ExecuteDeleteAsync(cancellationToken);
    }

    public void Remove(EmailVerification verification)
    {
        _context.EmailVerifications.Remove(verification);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
