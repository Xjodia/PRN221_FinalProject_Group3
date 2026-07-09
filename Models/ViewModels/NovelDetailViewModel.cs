namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class NovelDetailViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string AuthorName { get; init; } = string.Empty;

    public string TypeName { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string UpdatedAtText { get; init; } = string.Empty;

    public string Synopsis { get; init; } = string.Empty;

    public string CoverTheme { get; init; } = "violet";

    public string CoverSymbol { get; init; } = "✦";

    public string CoverImage { get; init; } = string.Empty;

    public int? FirstChapterId { get; init; }

    public IReadOnlyList<string> Categories { get; init; } = [];

    public IReadOnlyList<NovelStatisticViewModel> Statistics { get; init; } = [];

    public IReadOnlyList<NovelChapterItemViewModel> Chapters { get; init; } = [];

    public AuthorCardViewModel Author { get; init; } = new();

    public IReadOnlyList<RecommendedNovelViewModel> RecommendedNovels { get; init; } = [];

    public IReadOnlyList<CommentViewModel> Comments { get; init; } = [];
}

public class NovelStatisticViewModel
{
    public string Value { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public bool IsHighlight { get; init; }
}

public class NovelChapterItemViewModel
{
    public int Id { get; init; }

    public string Number { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string UpdatedAtText { get; init; } = string.Empty;

    public bool IsNew { get; init; }
}

public class AuthorCardViewModel
{
    public string DisplayName { get; init; } = string.Empty;

    public string Initials { get; init; } = string.Empty;

    public string Level { get; init; } = string.Empty;

    public string JoinedText { get; init; } = string.Empty;

    public string Bio { get; init; } = string.Empty;
}

public class RecommendedNovelViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Author { get; init; } = string.Empty;

    public string CoverTheme { get; init; } = "blue";

    public string CoverImage { get; init; } = string.Empty;

    public string RatingText { get; init; } = string.Empty;
}

public class CommentViewModel
{
    public string UserName { get; init; } = string.Empty;

    public string Initials { get; init; } = string.Empty;

    public string Level { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public string CreatedAtText { get; init; } = string.Empty;

    public bool IsPinned { get; init; }
}
