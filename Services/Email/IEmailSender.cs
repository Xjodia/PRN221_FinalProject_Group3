namespace PRN221_FinalProject_Group3.Services.Email;

public interface IEmailSender
{
    Task SendVerificationCodeAsync(
        string email,
        string code,
        string purpose,
        CancellationToken cancellationToken = default);
}
