namespace PRN221_FinalProject_Group3.Models.ViewModels;

public class ProfileViewModel
{
    public int Id { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string Initials { get; init; } = string.Empty;

    public string? AvatarUrl { get; init; }

    public bool IsInactive { get; init; }

    public string Bio { get; init; } = string.Empty;

    public string JoinedText { get; init; } = string.Empty;

    public int NovelCount { get; init; }

    public int ChapterCount { get; init; }

    public int CommentCount { get; init; }

    public IReadOnlyList<ProfileNovelViewModel> Novels { get; init; } = [];
}

public class ProfileNovelViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string CoverImage { get; init; } = string.Empty;

    public string StoryType { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string UpdatedText { get; init; } = string.Empty;

    public int ChapterCount { get; init; }
}
