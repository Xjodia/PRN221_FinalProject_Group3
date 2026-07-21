namespace PRN221_FinalProject_Group3.Services.Results;

public class AccountVerificationResult
{
    public bool Succeeded { get; private init; }

    public Guid? FlowId { get; private init; }

    public string? Email { get; private init; }

    public DateTimeOffset? ExpiresAt { get; private init; }

    public string? ResetToken { get; private init; }

    public bool IsExpired { get; private init; }

    public Dictionary<string, string> Errors { get; private init; } = new();

    public static AccountVerificationResult Success(
        Guid flowId,
        string? email = null,
        DateTimeOffset? expiresAt = null,
        string? resetToken = null)
    {
        return new AccountVerificationResult
        {
            Succeeded = true,
            FlowId = flowId,
            Email = email,
            ExpiresAt = expiresAt,
            ResetToken = resetToken
        };
    }

    public static AccountVerificationResult Failure(
        string key,
        string message,
        bool isExpired = false)
    {
        return new AccountVerificationResult
        {
            IsExpired = isExpired,
            Errors = new Dictionary<string, string>
            {
                [key] = message
            }
        };
    }

    public static AccountVerificationResult Failure(
        Dictionary<string, string> errors)
    {
        return new AccountVerificationResult
        {
            Errors = errors
        };
    }
}
