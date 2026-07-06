using System.ComponentModel.DataAnnotations;

namespace PRN221_FinalProject_Group3.Models;

public class ChapterComment
{
    public int Id { get; set; }

    public int ChapterId { get; set; }

    public int UserId { get; set; }

    public int? ParentCommentId { get; set; }

    [Required]
    [StringLength(3000)]
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Chapter Chapter { get; set; } = null!;

    public User User { get; set; } = null!;

    public ChapterComment? ParentComment { get; set; }

    public ICollection<ChapterComment> Replies { get; set; } = new List<ChapterComment>();
}
