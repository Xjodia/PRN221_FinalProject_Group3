namespace PRN221_FinalProject_Group3.Models;

public class ReadingHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int NovelId { get; set; }

    public int ChapterId { get; set; }

    public DateTimeOffset LastReadAt { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = null!;

    public Novel Novel { get; set; } = null!;

    public Chapter Chapter { get; set; } = null!;
}
