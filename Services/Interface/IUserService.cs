using PRN221_FinalProject_Group3.Models;
using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Interface;

public interface IUserService
{
    Task<AccountVerificationResult> StartRegistrationAsync(
        RegisterViewModel model,
        CancellationToken cancellationToken = default);

    Task<AccountVerificationResult> GetVerificationAsync(
        Guid flowId,
        EmailVerificationPurpose purpose,
        CancellationToken cancellationToken = default);

    Task<AccountVerificationResult> CompleteRegistrationAsync(
        Guid flowId,
        string code,
        CancellationToken cancellationToken = default);

    Task<AccountVerificationResult> StartPasswordResetAsync(
        ForgotPasswordViewModel model,
        CancellationToken cancellationToken = default);

    Task<AccountVerificationResult> VerifyPasswordResetCodeAsync(
        Guid flowId,
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> IsResetTokenValidAsync(
        Guid flowId,
        string token,
        CancellationToken cancellationToken = default);

    Task<RegisterResult> ResetPasswordAsync(
        ResetPasswordViewModel model,
        CancellationToken cancellationToken = default);

    Task<LoginResult> LoginAsync(
        LoginViewModel model,
        CancellationToken cancellationToken = default);

    Task<AccountSettingsViewModel?> GetAccountSettingsAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<RegisterResult> UpdateProfileAsync(
        ProfileEditViewModel model,
        int userId,
        CancellationToken cancellationToken = default);

    Task<RegisterResult> ChangePasswordAsync(
        ChangePasswordViewModel model,
        int userId,
        CancellationToken cancellationToken = default);
}
