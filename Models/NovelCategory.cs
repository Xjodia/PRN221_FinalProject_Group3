namespace PRN221_FinalProject_Group3.Models;

public class NovelCategory
{
    public int NovelId { get; set; }

    public int CategoryId { get; set; }

    public Novel Novel { get; set; } = null!;

    public Category Category { get; set; } = null!;
}
