# Data Model

## 1. Entity Relationship Diagram

```mermaid
erDiagram
    USER ||--o{ NOVEL : authors
    USER ||--o{ READING_HISTORY : has
    USER ||--o{ FOLLOW : creates
    USER ||--o{ NOVEL_COMMENT : writes
    USER ||--o{ CHAPTER_COMMENT : writes

    NOVEL ||--o{ CHAPTER : contains
    NOVEL ||--o{ NOVEL_CATEGORY : classified_as
    CATEGORY ||--o{ NOVEL_CATEGORY : groups
    NOVEL ||--o{ READING_HISTORY : appears_in
    CHAPTER ||--o{ READING_HISTORY : last_read
    NOVEL ||--o{ FOLLOW : followed_by
    NOVEL ||--o{ NOVEL_COMMENT : receives
    CHAPTER ||--o{ CHAPTER_COMMENT : receives

    NOVEL_COMMENT o|--o{ NOVEL_COMMENT : replies
    CHAPTER_COMMENT o|--o{ CHAPTER_COMMENT : replies

    USER {
        int Id PK
        string Username UK
        string Email UK
        string PasswordHash
        string DisplayName
        string Role
        string Status
        datetime CreatedAt
    }

    NOVEL {
        int Id PK
        int AuthorId FK
        string Title
        string Synopsis
        string Status
        bool IsActive
        long ViewCount
    }

    CHAPTER {
        int Id PK
        int NovelId FK
        decimal ChapterNumber
        string Title
        string Content
    }

    CATEGORY {
        int Id PK
        string Name UK
    }

    NOVEL_CATEGORY {
        int NovelId PK,FK
        int CategoryId PK,FK
    }

    READING_HISTORY {
        int Id PK
        int UserId FK
        int NovelId FK
        int ChapterId FK
        datetime LastReadAt
    }

    FOLLOW {
        int UserId PK,FK
        int NovelId PK,FK
        datetime CreatedAt
    }

    NOVEL_COMMENT {
        int Id PK
        int NovelId FK
        int UserId FK
        int ParentCommentId FK
        string Content
    }

    CHAPTER_COMMENT {
        int Id PK
        int ChapterId FK
        int UserId FK
        int ParentCommentId FK
        string Content
    }
```

## 2. Quy tắc dữ liệu quan trọng

- `User.Username` và `User.Email` là duy nhất.
- Mỗi cặp `NovelId + ChapterNumber` là duy nhất.
- `NovelCategory` dùng khóa chính ghép `NovelId + CategoryId`.
- `Follow` dùng khóa chính ghép `UserId + NovelId`.
- Mỗi người dùng chỉ có một bản ghi lịch sử cho mỗi truyện.
- Xóa `Novel` sẽ cascade đến `Chapter`.
- `Novel.IsActive` và `User.Status = Inactive` được dùng cho xóa mềm.
- Comment hỗ trợ cấu trúc cha-con qua `ParentCommentId`.

