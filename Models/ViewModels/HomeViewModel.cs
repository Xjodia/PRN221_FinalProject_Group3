namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class HomeViewModel
{
    public IReadOnlyList<NovelCardViewModel> FeaturedNovels { get; init; } = [];

    public IReadOnlyList<LatestChapterViewModel> LatestChapters { get; init; } = [];

    public IReadOnlyList<PopularNovelViewModel> PopularNovels { get; init; } = [];

    public IReadOnlyList<string> Categories { get; init; } = [];
}

public class NovelCardViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Author { get; init; } = string.Empty;

    public string LatestChapter { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string CoverTheme { get; init; } = "violet";

    public long ViewCount { get; init; }
}

public class LatestChapterViewModel
{
    public string NovelTitle { get; init; } = string.Empty;

    public string ChapterTitle { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string UpdatedText { get; init; } = string.Empty;
}

public class PopularNovelViewModel
{
    public int Rank { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Author { get; init; } = string.Empty;

    public long ViewCount { get; init; }
}
