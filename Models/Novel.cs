using System.ComponentModel.DataAnnotations;

namespace PRN221_FinalProject_Group3.Models;

public class Novel
{
    public int Id { get; set; }

    [Required]
    [StringLength(250)]
    public string Title { get; set; } = string.Empty;

    public int AuthorId { get; set; }

    [StringLength(500)]
    public string? OtherNames { get; set; }

    [StringLength(150)]
    public string? OriginalAuthor { get; set; }

    [StringLength(150)]
    public string? Illustrator { get; set; }

    [StringLength(50)]
    public string StoryType { get; set; } = "Truyện dịch";

    [StringLength(150)]
    public string? TranslationGroup { get; set; }

    [Required]
    public string Synopsis { get; set; } = string.Empty;

    public string? Note { get; set; }

    [StringLength(500)]
    public string? CoverImage { get; set; }

    public NovelStatus Status { get; set; } = NovelStatus.Ongoing;

    public bool IsActive { get; set; } = true;

    public long ViewCount { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public User Author { get; set; } = null!;

    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public ICollection<NovelCategory> NovelCategories { get; set; } = new List<NovelCategory>();

    public ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();

    public ICollection<Follow> Followers { get; set; } = new List<Follow>();

    public ICollection<NovelComment> Comments { get; set; } = new List<NovelComment>();
}

public enum NovelStatus
{
    Ongoing = 0,
    Completed = 1,
    Paused = 2
}
