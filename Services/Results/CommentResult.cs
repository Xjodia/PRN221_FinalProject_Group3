namespace PRN221_FinalProject_Group3.Services.Results;

public class CommentResult
{
    public bool Succeeded { get; init; }

    public string? Error { get; init; }

    public static CommentResult Success()
    {
        return new CommentResult { Succeeded = true };
    }

    public static CommentResult Failure(string error)
    {
        return new CommentResult
        {
            Succeeded = false,
            Error = error
        };
    }
}
