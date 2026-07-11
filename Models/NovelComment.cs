using System.ComponentModel.DataAnnotations;

namespace PRN221_FinalProject_Group3.Models;

public class NovelComment
{
    public int Id { get; set; }

    public int NovelId { get; set; }

    public int UserId { get; set; }

    public int? ParentCommentId { get; set; }

    [Required]
    [StringLength(3000)]
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Novel Novel { get; set; } = null!;

    public User User { get; set; } = null!;

    public NovelComment? ParentComment { get; set; }

    public ICollection<NovelComment> Replies { get; set; } = new List<NovelComment>();
}
