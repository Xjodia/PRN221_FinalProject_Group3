namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class SystemHomeViewModel
{
    public int ActiveNovelCount { get; init; }

    public int DeletedNovelCount { get; init; }

    public int UserCount { get; init; }
}

public class SystemNovelListViewModel
{
    public string? SearchTerm { get; init; }

    public bool ShowDeleted { get; init; }

    public IReadOnlyList<SystemNovelItemViewModel> Novels { get; init; } = [];
}

public class SystemNovelItemViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string AuthorName { get; init; } = string.Empty;

    public string? TranslationGroup { get; init; }

    public int ChapterCount { get; init; }

    public string UpdatedAtText { get; init; } = string.Empty;
}
