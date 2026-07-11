namespace PRN221_FinalProject_Group3.Services.Results;

public class ManagementResult
{
    public bool Succeeded { get; init; }

    public int? EntityId { get; init; }

    public Dictionary<string, string> Errors { get; init; } = new();

    public static ManagementResult Success(int? entityId = null)
    {
        return new ManagementResult
        {
            Succeeded = true,
            EntityId = entityId
        };
    }

    public static ManagementResult Failure(string key, string error)
    {
        return new ManagementResult
        {
            Succeeded = false,
            Errors = new Dictionary<string, string>
            {
                [key] = error
            }
        };
    }
}
