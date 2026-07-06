namespace PRN221_FinalProject_Group3.Models;

public class Follow
{
    public int UserId { get; set; }

    public int NovelId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = null!;

    public Novel Novel { get; set; } = null!;
}
