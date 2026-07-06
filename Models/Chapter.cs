using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN221_FinalProject_Group3.Models;

public class Chapter
{
    public int Id { get; set; }

    public int NovelId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal ChapterNumber { get; set; }

    [Required]
    [StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Novel Novel { get; set; } = null!;

    public ICollection<ChapterComment> Comments { get; set; } = new List<ChapterComment>();

    public ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();
}
