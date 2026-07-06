IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706015147_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [PasswordHash] nvarchar(255) NOT NULL,
        [DisplayName] nvarchar(100) NOT NULL,
        [AvatarUrl] nvarchar(500) NULL,
        [Bio] nvarchar(500) NULL,
        [Role] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [IsEmailVerified] bit NOT NULL,
        [LastLoginAt] datetimeoffset NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706015147_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706015147_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706015147_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260706015147_InitialCreate', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE TABLE [Novels] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(250) NOT NULL,
        [AuthorId] int NOT NULL,
        [Synopsis] nvarchar(max) NOT NULL,
        [CoverImage] nvarchar(500) NULL,
        [Status] nvarchar(20) NOT NULL,
        [ViewCount] bigint NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_Novels] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Novels_Users_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE TABLE [Chapters] (
        [Id] int NOT NULL IDENTITY,
        [NovelId] int NOT NULL,
        [ChapterNumber] decimal(10,2) NOT NULL,
        [Title] nvarchar(250) NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_Chapters] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Chapters_Novels_NovelId] FOREIGN KEY ([NovelId]) REFERENCES [Novels] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE TABLE [Follows] (
        [UserId] int NOT NULL,
        [NovelId] int NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_Follows] PRIMARY KEY ([UserId], [NovelId]),
        CONSTRAINT [FK_Follows_Novels_NovelId] FOREIGN KEY ([NovelId]) REFERENCES [Novels] ([Id]),
        CONSTRAINT [FK_Follows_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE TABLE [NovelCategories] (
        [NovelId] int NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_NovelCategories] PRIMARY KEY ([NovelId], [CategoryId]),
        CONSTRAINT [FK_NovelCategories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_NovelCategories_Novels_NovelId] FOREIGN KEY ([NovelId]) REFERENCES [Novels] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE TABLE [ChapterComments] (
        [Id] int NOT NULL IDENTITY,
        [ChapterId] int NOT NULL,
        [UserId] int NOT NULL,
        [ParentCommentId] int NULL,
        [Content] nvarchar(3000) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_ChapterComments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChapterComments_ChapterComments_ParentCommentId] FOREIGN KEY ([ParentCommentId]) REFERENCES [ChapterComments] ([Id]),
        CONSTRAINT [FK_ChapterComments_Chapters_ChapterId] FOREIGN KEY ([ChapterId]) REFERENCES [Chapters] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ChapterComments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE TABLE [ReadingHistories] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [NovelId] int NOT NULL,
        [ChapterId] int NOT NULL,
        [LastReadAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_ReadingHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ReadingHistories_Chapters_ChapterId] FOREIGN KEY ([ChapterId]) REFERENCES [Chapters] ([Id]),
        CONSTRAINT [FK_ReadingHistories_Novels_NovelId] FOREIGN KEY ([NovelId]) REFERENCES [Novels] ([Id]),
        CONSTRAINT [FK_ReadingHistories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Categories_Name] ON [Categories] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_ChapterComments_ChapterId] ON [ChapterComments] ([ChapterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_ChapterComments_ParentCommentId] ON [ChapterComments] ([ParentCommentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_ChapterComments_UserId] ON [ChapterComments] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Chapters_NovelId_ChapterNumber] ON [Chapters] ([NovelId], [ChapterNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_Follows_CreatedAt] ON [Follows] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_Follows_NovelId] ON [Follows] ([NovelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_NovelCategories_CategoryId] ON [NovelCategories] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_Novels_AuthorId] ON [Novels] ([AuthorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_Novels_Title] ON [Novels] ([Title]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_ReadingHistories_ChapterId] ON [ReadingHistories] ([ChapterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_ReadingHistories_LastReadAt] ON [ReadingHistories] ([LastReadAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE INDEX [IX_ReadingHistories_NovelId] ON [ReadingHistories] ([NovelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ReadingHistories_UserId_NovelId] ON [ReadingHistories] ([UserId], [NovelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260706031457_AddNovelReadingFeatures'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260706031457_AddNovelReadingFeatures', N'8.0.0');
END;
GO

COMMIT;
GO

