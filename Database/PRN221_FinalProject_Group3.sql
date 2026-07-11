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
        [OtherNames] nvarchar(500) NULL,
        [OriginalAuthor] nvarchar(150) NULL,
        [Illustrator] nvarchar(150) NULL,
        [StoryType] nvarchar(50) NOT NULL DEFAULT N'Truyện dịch',
        [TranslationGroup] nvarchar(150) NULL,
        [Synopsis] nvarchar(max) NOT NULL,
        [Note] nvarchar(max) NULL,
        [CoverImage] nvarchar(500) NULL,
        [Status] nvarchar(20) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [ViewCount] bigint NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_Novels] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Novels_Users_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF COL_LENGTH('Novels', 'OtherNames') IS NULL
BEGIN
    ALTER TABLE [Novels] ADD [OtherNames] nvarchar(500) NULL;
END;
GO

IF COL_LENGTH('Novels', 'OriginalAuthor') IS NULL
BEGIN
    ALTER TABLE [Novels] ADD [OriginalAuthor] nvarchar(150) NULL;
END;
GO

IF COL_LENGTH('Novels', 'Illustrator') IS NULL
BEGIN
    ALTER TABLE [Novels] ADD [Illustrator] nvarchar(150) NULL;
END;
GO

IF COL_LENGTH('Novels', 'StoryType') IS NULL
BEGIN
    ALTER TABLE [Novels] ADD [StoryType] nvarchar(50) NOT NULL CONSTRAINT [DF_Novels_StoryType] DEFAULT N'Truyện dịch';
END;
GO

IF COL_LENGTH('Novels', 'IsActive') IS NULL
BEGIN
    ALTER TABLE [Novels] ADD [IsActive] bit NOT NULL CONSTRAINT [DF_Novels_IsActive] DEFAULT CAST(1 AS bit);
END;
GO

IF COL_LENGTH('Novels', 'TranslationGroup') IS NULL
BEGIN
    ALTER TABLE [Novels] ADD [TranslationGroup] nvarchar(150) NULL;
END;
GO

IF COL_LENGTH('Novels', 'Note') IS NULL
BEGIN
    ALTER TABLE [Novels] ADD [Note] nvarchar(max) NULL;
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

/* =========================================================
   Seed data for StoryNest home/detail pages
   Safe to run multiple times.
   ========================================================= */

DECLARE @CoverImage nvarchar(500) = N'https://i.imgur.com/FTAaZvy.jpeg';
DECLARE @ChapterImage nvarchar(500) = N'https://acacia.wiki.gg/images/thumb/Yuki_end_01.png/1920px-Yuki_end_01.png?92c92d';
DECLARE @Now datetimeoffset = SYSDATETIMEOFFSET();

IF OBJECT_ID(N'[NovelComments]', N'U') IS NULL
BEGIN
    CREATE TABLE [NovelComments] (
        [Id] int NOT NULL IDENTITY,
        [NovelId] int NOT NULL,
        [UserId] int NOT NULL,
        [ParentCommentId] int NULL,
        [Content] nvarchar(3000) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_NovelComments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NovelComments_Novels_NovelId] FOREIGN KEY ([NovelId]) REFERENCES [Novels] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_NovelComments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]),
        CONSTRAINT [FK_NovelComments_NovelComments_ParentCommentId] FOREIGN KEY ([ParentCommentId]) REFERENCES [NovelComments] ([Id])
    );

    CREATE INDEX [IX_NovelComments_NovelId] ON [NovelComments] ([NovelId]);
    CREATE INDEX [IX_NovelComments_UserId] ON [NovelComments] ([UserId]);
    CREATE INDEX [IX_NovelComments_ParentCommentId] ON [NovelComments] ([ParentCommentId]);
END;

IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Username] = N'seed_author')
BEGIN
    INSERT INTO [Users]
        ([Username], [Email], [PasswordHash], [DisplayName], [AvatarUrl], [Bio], [Role], [Status], [IsEmailVerified], [LastLoginAt], [CreatedAt], [UpdatedAt])
    VALUES
        (N'seed_author', N'seed.author@storynest.local', N'seeded-account-not-for-login', N'Lạc Gia Tiểu Mạt', NULL,
         N'Tác giả yêu các motif xuyên sách, phản diện và những nhân vật có chút nguy hiểm.',
         N'Author', N'Active', 1, NULL, @Now, @Now);
END;

IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Username] = N'seed_reader')
BEGIN
    INSERT INTO [Users]
        ([Username], [Email], [PasswordHash], [DisplayName], [AvatarUrl], [Bio], [Role], [Status], [IsEmailVerified], [LastLoginAt], [CreatedAt], [UpdatedAt])
    VALUES
        (N'seed_reader', N'seed.reader@storynest.local', N'seeded-account-not-for-login', N'Gwee', NULL,
         N'Độc giả thích để lại bình luận vui vẻ ở mỗi chương.',
         N'Reader', N'Active', 1, NULL, @Now, @Now);
END;

IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Username] = N'test_admin')
BEGIN
    INSERT INTO [Users]
        ([Username], [Email], [PasswordHash], [DisplayName], [AvatarUrl], [Bio], [Role], [Status], [IsEmailVerified], [LastLoginAt], [CreatedAt], [UpdatedAt])
    VALUES
        (N'test_admin', N'test.admin@storynest.local', N'$2a$11$ei.E5Cjy.qnh9JyDmx1OJOkSThBgxuSQMjHimJW8ieUgcHaGcbW4a', N'Admin Test', NULL,
         N'Tài khoản admin dùng để kiểm thử hệ thống.',
         N'Admin', N'Active', 1, NULL, @Now, @Now);
END;

IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [Name] = N'Yandere') INSERT INTO [Categories] ([Name]) VALUES (N'Yandere');
IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [Name] = N'Female Protagonist') INSERT INTO [Categories] ([Name]) VALUES (N'Female Protagonist');
IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [Name] = N'Drama') INSERT INTO [Categories] ([Name]) VALUES (N'Drama');
IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [Name] = N'Gender Bender') INSERT INTO [Categories] ([Name]) VALUES (N'Gender Bender');
IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [Name] = N'Psychological') INSERT INTO [Categories] ([Name]) VALUES (N'Psychological');
IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [Name] = N'Mature') INSERT INTO [Categories] ([Name]) VALUES (N'Mature');
IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [Name] = N'Fantasy') INSERT INTO [Categories] ([Name]) VALUES (N'Fantasy');
IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [Name] = N'Comedy') INSERT INTO [Categories] ([Name]) VALUES (N'Comedy');
IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [Name] = N'Romance') INSERT INTO [Categories] ([Name]) VALUES (N'Romance');
IF NOT EXISTS (SELECT 1 FROM [Categories] WHERE [Name] = N'Adventure') INSERT INTO [Categories] ([Name]) VALUES (N'Adventure');

DECLARE @AuthorId int = (SELECT [Id] FROM [Users] WHERE [Username] = N'seed_author');

IF NOT EXISTS (SELECT 1 FROM [Novels] WHERE [Title] = N'Xuyên thành một nữ phụ phản diện? Tôi đã trở thành 1 loli yandere')
BEGIN
    INSERT INTO [Novels] ([Title], [AuthorId], [Synopsis], [CoverImage], [Status], [ViewCount], [CreatedAt], [UpdatedAt])
    VALUES
    (
        N'Xuyên thành một nữ phụ phản diện? Tôi đã trở thành 1 loli yandere',
        @AuthorId,
        N'Tri Hiểu Thanh không thể tin nổi.

Cô vậy mà lại xuyên vào một cuốn tiểu thuyết ngôn tình mất não!

Không những thế, cô còn trở thành một nữ phản diện lắm nền. Vừa mở màn đã cắm sẵn một cái death flag, Tri Hiểu Thanh quyết định hóa thân thành một loli yandere và tỏ tình với nữ chính.

“Đào Đào, người em yêu chính là chị!”

“Chị nói chị yêu em, vậy mà tại sao lại còn nhìn người khác?!”',
        @CoverImage,
        N'Ongoing',
        12890,
        DATEADD(day, -18, @Now),
        DATEADD(day, -1, @Now)
    );
END;

IF NOT EXISTS (SELECT 1 FROM [Novels] WHERE [Title] = N'Arknights: Nhóm Chat Của Các Game Thủ')
BEGIN
    INSERT INTO [Novels] ([Title], [AuthorId], [Synopsis], [CoverImage], [Status], [ViewCount], [CreatedAt], [UpdatedAt])
    VALUES
    (N'Arknights: Nhóm Chat Của Các Game Thủ', @AuthorId,
     N'Một nhóm game thủ bất ngờ bị cuốn vào thế giới hỗn loạn, nơi mỗi tin nhắn đều có thể thay đổi vận mệnh.',
     @CoverImage, N'Ongoing', 10620, DATEADD(day, -32, @Now), DATEADD(day, -2, @Now));
END;

IF NOT EXISTS (SELECT 1 FROM [Novels] WHERE [Title] = N'Vô Lâm Dị Khách')
BEGIN
    INSERT INTO [Novels] ([Title], [AuthorId], [Synopsis], [CoverImage], [Status], [ViewCount], [CreatedAt], [UpdatedAt])
    VALUES
    (N'Vô Lâm Dị Khách', @AuthorId,
     N'Một kẻ lữ hành vô danh bước vào giang hồ, mang theo bí mật có thể làm lung lay cả võ lâm.',
     @CoverImage, N'Ongoing', 9810, DATEADD(day, -28, @Now), DATEADD(day, -3, @Now));
END;

IF NOT EXISTS (SELECT 1 FROM [Novels] WHERE [Title] = N'Cô vợ đa nhân cách quá yêu tôi')
BEGIN
    INSERT INTO [Novels] ([Title], [AuthorId], [Synopsis], [CoverImage], [Status], [ViewCount], [CreatedAt], [UpdatedAt])
    VALUES
    (N'Cô vợ đa nhân cách quá yêu tôi', @AuthorId,
     N'Mỗi sáng thức dậy, cô ấy lại là một con người khác. Nhưng dù là ai, cô ấy vẫn luôn chọn yêu tôi.',
     @CoverImage, N'Ongoing', 8750, DATEADD(day, -22, @Now), DATEADD(day, -4, @Now));
END;

IF NOT EXISTS (SELECT 1 FROM [Novels] WHERE [Title] = N'Người giữ thư viện cuối cùng')
BEGIN
    INSERT INTO [Novels] ([Title], [AuthorId], [Synopsis], [CoverImage], [Status], [ViewCount], [CreatedAt], [UpdatedAt])
    VALUES
    (N'Người giữ thư viện cuối cùng', @AuthorId,
     N'Khi tri thức trở thành thứ bị săn đuổi, một cô gái trẻ quyết định bảo vệ thư viện cuối cùng của nhân loại.',
     @CoverImage, N'Ongoing', 7420, DATEADD(day, -41, @Now), DATEADD(day, -5, @Now));
END;

IF NOT EXISTS (SELECT 1 FROM [Novels] WHERE [Title] = N'Sau khi trọng sinh tôi chỉ muốn đọc sách')
BEGIN
    INSERT INTO [Novels] ([Title], [AuthorId], [Synopsis], [CoverImage], [Status], [ViewCount], [CreatedAt], [UpdatedAt])
    VALUES
    (N'Sau khi trọng sinh tôi chỉ muốn đọc sách', @AuthorId,
     N'Được sống lại lần nữa, cô chỉ muốn yên ổn đọc sách. Nhưng số phận hình như không thích sự yên ổn.',
     @CoverImage, N'Completed', 6980, DATEADD(day, -55, @Now), DATEADD(day, -6, @Now));
END;

DECLARE @NovelId int;
DECLARE @CategoryId int;

DECLARE novel_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT [Id] FROM [Novels]
WHERE [Title] IN
(
    N'Xuyên thành một nữ phụ phản diện? Tôi đã trở thành 1 loli yandere',
    N'Arknights: Nhóm Chat Của Các Game Thủ',
    N'Vô Lâm Dị Khách',
    N'Cô vợ đa nhân cách quá yêu tôi',
    N'Người giữ thư viện cuối cùng',
    N'Sau khi trọng sinh tôi chỉ muốn đọc sách'
);

OPEN novel_cursor;
FETCH NEXT FROM novel_cursor INTO @NovelId;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE category_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT [Id] FROM [Categories]
    WHERE [Name] IN (N'Yandere', N'Female Protagonist', N'Drama', N'Gender Bender', N'Psychological', N'Mature', N'Fantasy', N'Comedy', N'Romance', N'Adventure');

    OPEN category_cursor;
    FETCH NEXT FROM category_cursor INTO @CategoryId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM [NovelCategories] WHERE [NovelId] = @NovelId AND [CategoryId] = @CategoryId)
        BEGIN
            INSERT INTO [NovelCategories] ([NovelId], [CategoryId]) VALUES (@NovelId, @CategoryId);
        END;

        FETCH NEXT FROM category_cursor INTO @CategoryId;
    END;

    CLOSE category_cursor;
    DEALLOCATE category_cursor;

    IF NOT EXISTS (SELECT 1 FROM [Chapters] WHERE [NovelId] = @NovelId AND [ChapterNumber] = 1)
    BEGIN
        INSERT INTO [Chapters] ([NovelId], [ChapterNumber], [Title], [Content], [CreatedAt], [UpdatedAt])
        VALUES
        (@NovelId, 1, N'Nữ thần toàn trí toàn năng',
         N'Đêm ấy, cô mở mắt giữa căn phòng xa lạ. Những ký ức rời rạc của nhân vật phản diện tràn vào tâm trí như một cuốn sách bị xé vụn.',
         DATEADD(day, -8, @Now), DATEADD(day, -4, @Now));
    END;

    IF NOT EXISTS (SELECT 1 FROM [Chapters] WHERE [NovelId] = @NovelId AND [ChapterNumber] = 2)
    BEGIN
        INSERT INTO [Chapters] ([NovelId], [ChapterNumber], [Title], [Content], [CreatedAt], [UpdatedAt])
        VALUES
        (@NovelId, 2, N'Cô gái lạ mặt',
         N'Một nụ cười quá dịu dàng thường là điềm báo cho tai họa. Tri Hiểu Thanh biết điều đó, nhưng vẫn không thể quay đầu.',
         DATEADD(day, -7, @Now), DATEADD(day, -2, @Now));
    END;

    FETCH NEXT FROM novel_cursor INTO @NovelId;
END;

CLOSE novel_cursor;
DEALLOCATE novel_cursor;

UPDATE [Chapters]
SET [Content] = N'<p>' + [Content] + N'</p>'
WHERE [Content] NOT LIKE N'<%'
  AND [NovelId] IN
  (
      SELECT [Id]
      FROM [Novels]
      WHERE [Title] IN
      (
          N'Xuyên thành một nữ phụ phản diện? Tôi đã trở thành 1 loli yandere',
          N'Arknights: Nhóm Chat Của Các Game Thủ',
          N'Vô Lâm Dị Khách',
          N'Cô vợ đa nhân cách quá yêu tôi',
          N'Người giữ thư viện cuối cùng',
          N'Sau khi trọng sinh tôi chỉ muốn đọc sách'
      )
  );

DECLARE @MainNovelId int = (SELECT [Id] FROM [Novels] WHERE [Title] = N'Xuyên thành một nữ phụ phản diện? Tôi đã trở thành 1 loli yandere');
DECLARE @MainChapterId int = (SELECT TOP 1 [Id] FROM [Chapters] WHERE [NovelId] = @MainNovelId ORDER BY [ChapterNumber]);
DECLARE @ReaderId int = (SELECT [Id] FROM [Users] WHERE [Username] = N'seed_reader');

UPDATE [Chapters]
SET [Content] = N'<p><em>Ào—</em></p>
<p>Nước lạnh buốt thấu tim trút ào từ chiếc xô sắt xuống. Cái lạnh cắt da men theo mái tóc dài bạc trắng rồi nhanh chóng thấm ướt bộ đồng phục giản dị trên người thiếu nữ.</p>
<p>Những dòng nước chảy ngoằn ngoèo trên gò má cô, còn màu đồng phục cũng vì ngấm nước mà sẫm xuống mấy phần. Chiếc quần tất trắng vốn đã mang lại cảm giác bí bức cùng đôi bốt da đen lập tức hút đầy nước. Sau khoảnh khắc mát lạnh ngắn ngủi, chúng chỉ khiến cảm giác ngột ngạt quanh người càng trở nên rõ rệt hơn.</p>
<p>“Lạnh... thật.”</p>
<p>Bị đối xử như vậy, thiếu nữ vẫn không hề nổi giận mà chỉ bình thản nói ra cảm nhận của mình lúc này.</p>
<p><img src="' + @ChapterImage + N'" alt="Minh họa chương 1" /></p>
<p>“Lạnh là đúng rồi. Bị dội nước lạnh mà không lạnh mới lạ. Nhưng cô không thể thấy giận hơn một chút sao? Không thể lộ ra vẻ tức tối hơn một chút à?”</p>
<p>Đứng đối diện cô là thủ phạm mang tên Utsunomiya Hoshino. Cậu thiếu niên vẫn đang xách chiếc xô sắt đã trút sạch nước và nhìn cô với vẻ khó hiểu đến cực điểm.</p>
<p>“Sao cô vẫn cứ trưng cái mặt người chết mà còn cười tủm tỉm ấy ra vậy?”</p>',
    [UpdatedAt] = DATEADD(day, -1, @Now)
WHERE [ChapterNumber] = 1
  AND [NovelId] IN
  (
      SELECT [Id]
      FROM [Novels]
      WHERE [Title] IN
      (
          N'Xuyên thành một nữ phụ phản diện? Tôi đã trở thành 1 loli yandere',
          N'Arknights: Nhóm Chat Của Các Game Thủ',
          N'Vô Lâm Dị Khách',
          N'Cô vợ đa nhân cách quá yêu tôi',
          N'Người giữ thư viện cuối cùng',
          N'Sau khi trọng sinh tôi chỉ muốn đọc sách'
      )
  );

IF @MainChapterId IS NOT NULL
BEGIN
    UPDATE [ChapterComments]
    SET [Content] = N'<p>Đã edit lại.</p><p>Dùng AI có thể dịch sai, mọi người thấy chỗ nào cấn thì nhắc mình nhé.</p>'
    WHERE [ChapterId] = @MainChapterId AND [UserId] = @AuthorId AND [Content] = N'Đã edit lại.';

    UPDATE [ChapterComments]
    SET [Content] = N'<p>Ể nha, “người bạn thân Hiểu Thanh” nghe có mùi drama rồi đó.</p>'
    WHERE [ChapterId] = @MainChapterId AND [UserId] = @ReaderId AND [Content] = N'Ể nha, “người bạn thân Hiểu Thanh” nghe có mùi drama rồi đó.';

    IF NOT EXISTS (SELECT 1 FROM [ChapterComments] WHERE [ChapterId] = @MainChapterId AND [UserId] = @AuthorId AND [Content] = N'<p>Đã edit lại.</p><p>Dùng AI có thể dịch sai, mọi người thấy chỗ nào cấn thì nhắc mình nhé.</p>')
    BEGIN
        INSERT INTO [ChapterComments] ([ChapterId], [UserId], [ParentCommentId], [Content], [CreatedAt], [UpdatedAt])
        VALUES (@MainChapterId, @AuthorId, NULL, N'<p>Đã edit lại.</p><p>Dùng AI có thể dịch sai, mọi người thấy chỗ nào cấn thì nhắc mình nhé.</p>', DATEADD(day, -5, @Now), DATEADD(day, -5, @Now));
    END;

    IF NOT EXISTS (SELECT 1 FROM [ChapterComments] WHERE [ChapterId] = @MainChapterId AND [UserId] = @ReaderId AND [Content] = N'<p>Ể nha, “người bạn thân Hiểu Thanh” nghe có mùi drama rồi đó.</p>')
    BEGIN
        INSERT INTO [ChapterComments] ([ChapterId], [UserId], [ParentCommentId], [Content], [CreatedAt], [UpdatedAt])
        VALUES (@MainChapterId, @ReaderId, NULL, N'<p>Ể nha, “người bạn thân Hiểu Thanh” nghe có mùi drama rồi đó.</p>', DATEADD(day, -4, @Now), DATEADD(day, -4, @Now));
    END;

    DECLARE @OwnerCommentId int = (
        SELECT TOP 1 [Id]
        FROM [ChapterComments]
        WHERE [ChapterId] = @MainChapterId AND [UserId] = @AuthorId
        ORDER BY [Id]
    );

    IF @OwnerCommentId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM [ChapterComments] WHERE [ParentCommentId] = @OwnerCommentId AND [UserId] = @ReaderId)
    BEGIN
        INSERT INTO [ChapterComments] ([ChapterId], [UserId], [ParentCommentId], [Content], [CreatedAt], [UpdatedAt])
        VALUES (@MainChapterId, @ReaderId, @OwnerCommentId, N'<p>Ok chủ thớt, để mình đọc kỹ rồi góp ý sau.</p>', DATEADD(day, -3, @Now), DATEADD(day, -3, @Now));
    END;
END;

IF @MainNovelId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [NovelComments] WHERE [NovelId] = @MainNovelId AND [UserId] = @AuthorId AND [Content] = N'<p>Đây là khu bình luận tổng của truyện, không gắn vào chương nào.</p>')
    BEGIN
        INSERT INTO [NovelComments] ([NovelId], [UserId], [ParentCommentId], [Content], [CreatedAt], [UpdatedAt])
        VALUES (@MainNovelId, @AuthorId, NULL, N'<p>Đây là khu bình luận tổng của truyện, không gắn vào chương nào.</p>', DATEADD(day, -2, @Now), DATEADD(day, -2, @Now));
    END;

    IF NOT EXISTS (SELECT 1 FROM [NovelComments] WHERE [NovelId] = @MainNovelId AND [UserId] = @ReaderId AND [Content] = N'<p>Ok, vậy comment ở detail sẽ nằm riêng ngoài truyện.</p>')
    BEGIN
        INSERT INTO [NovelComments] ([NovelId], [UserId], [ParentCommentId], [Content], [CreatedAt], [UpdatedAt])
        VALUES (@MainNovelId, @ReaderId, NULL, N'<p>Ok, vậy comment ở detail sẽ nằm riêng ngoài truyện.</p>', DATEADD(day, -1, @Now), DATEADD(day, -1, @Now));
    END;

    DECLARE @NovelOwnerCommentId int = (
        SELECT TOP 1 [Id]
        FROM [NovelComments]
        WHERE [NovelId] = @MainNovelId AND [UserId] = @AuthorId
        ORDER BY [Id]
    );

    IF @NovelOwnerCommentId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM [NovelComments] WHERE [ParentCommentId] = @NovelOwnerCommentId AND [UserId] = @ReaderId)
    BEGIN
        INSERT INTO [NovelComments] ([NovelId], [UserId], [ParentCommentId], [Content], [CreatedAt], [UpdatedAt])
        VALUES (@MainNovelId, @ReaderId, @NovelOwnerCommentId, N'<p>Reply ngoài detail cũng hoạt động riêng.</p>', DATEADD(hour, -12, @Now), DATEADD(hour, -12, @Now));
    END;
END;

UPDATE [Novels]
SET [CoverImage] = @CoverImage
WHERE [Title] IN
(
    N'Xuyên thành một nữ phụ phản diện? Tôi đã trở thành 1 loli yandere',
    N'Arknights: Nhóm Chat Của Các Game Thủ',
    N'Vô Lâm Dị Khách',
    N'Cô vợ đa nhân cách quá yêu tôi',
    N'Người giữ thư viện cuối cùng',
    N'Sau khi trọng sinh tôi chỉ muốn đọc sách'
);
GO

