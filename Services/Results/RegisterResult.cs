namespace PRN221_FinalProject_Group3.Services.Results;

public class RegisterResult
{
    public bool Succeeded { get; private init; }

    public Dictionary<string, string> Errors { get; private init; } = new();

    public static RegisterResult Success()
    {
        return new RegisterResult { Succeeded = true };
    }

    public static RegisterResult Failure(string key, string message)
    {
        return Failure(new Dictionary<string, string>
        {
            [key] = message
        });
    }

    public static RegisterResult Failure(Dictionary<string, string> errors)
    {
        return new RegisterResult
        {
            Succeeded = false,
            Errors = errors
        };
    }
}
