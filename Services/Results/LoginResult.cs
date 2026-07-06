using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.Services.Results;

public class LoginResult
{
    public bool Succeeded { get; private init; }

    public User? User { get; private init; }

    public string? Error { get; private init; }

    public static LoginResult Success(User user)
    {
        return new LoginResult
        {
            Succeeded = true,
            User = user
        };
    }

    public static LoginResult Failure(string error)
    {
        return new LoginResult
        {
            Succeeded = false,
            Error = error
        };
    }
}
