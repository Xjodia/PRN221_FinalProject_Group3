using System.ComponentModel.DataAnnotations;

namespace PRN221_FinalProject_Group3.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<NovelCategory> NovelCategories { get; set; } = new List<NovelCategory>();
}
