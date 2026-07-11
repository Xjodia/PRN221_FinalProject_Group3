namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class FollowedNovelsViewModel
{
    public string? SearchTerm { get; init; }

    public IReadOnlyList<FollowedNovelItemViewModel> Novels { get; init; } = [];
}

public class FollowedNovelItemViewModel
{
    public int NovelId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string AuthorName { get; init; } = string.Empty;

    public string CoverImage { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public int? LatestChapterId { get; init; }

    public string LatestChapterTitle { get; init; } = string.Empty;

    public string FollowedAtText { get; init; } = string.Empty;
}

public class ReadingHistoryViewModel
{
    public IReadOnlyList<ReadingHistoryItemViewModel> Items { get; init; } = [];
}

public class ReadingHistoryItemViewModel
{
    public int NovelId { get; init; }

    public string NovelTitle { get; init; } = string.Empty;

    public int ChapterId { get; init; }

    public string ChapterTitle { get; init; } = string.Empty;

    public string LastReadAtText { get; init; } = string.Empty;
}
