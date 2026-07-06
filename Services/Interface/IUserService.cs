using PRN221_FinalProject_Group3.Models.ViewModels;
using PRN221_FinalProject_Group3.Services.Results;

namespace PRN221_FinalProject_Group3.Services.Interface;

public interface IUserService
{
    Task<RegisterResult> RegisterUserAsync(
        RegisterViewModel model,
        CancellationToken cancellationToken = default);

    Task<LoginResult> LoginAsync(
        LoginViewModel model,
        CancellationToken cancellationToken = default);
}
