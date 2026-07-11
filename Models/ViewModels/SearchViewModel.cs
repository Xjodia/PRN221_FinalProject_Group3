namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class SearchViewModel
{
    public string Query { get; init; } = string.Empty;

    public string Tab { get; init; } = "all";

    public string Status { get; init; } = "all";

    public string Sort { get; init; } = "relevance";

    public int TotalCount => NovelResults.Count + MemberResults.Count;

    public IReadOnlyList<SearchNovelResultViewModel> NovelResults { get; init; } = [];

    public IReadOnlyList<SearchMemberResultViewModel> MemberResults { get; init; } = [];
}

public class SearchNovelResultViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Author { get; init; } = string.Empty;

    public string Synopsis { get; init; } = string.Empty;

    public string CoverImage { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public long ViewCount { get; init; }

    public int ChapterCount { get; init; }

    public string UpdatedText { get; init; } = string.Empty;
}

public class SearchMemberResultViewModel
{
    public int Id { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string Initials { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public string Bio { get; init; } = string.Empty;

    public int NovelCount { get; init; }
}
