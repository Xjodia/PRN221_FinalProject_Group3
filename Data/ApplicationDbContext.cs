using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject_Group3.Models;

namespace PRN221_FinalProject_Group3.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Novel> Novels => Set<Novel>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<NovelCategory> NovelCategories => Set<NovelCategory>();
    public DbSet<ReadingHistory> ReadingHistories => Set<ReadingHistory>();
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<ChapterComment> ChapterComments => Set<ChapterComment>();
    public DbSet<NovelComment> NovelComments => Set<NovelComment>();
    public DbSet<EmailVerification> EmailVerifications => Set<EmailVerification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(user => user.Username).IsUnique();
            entity.HasIndex(user => user.Email).IsUnique();

            entity.Property(user => user.Role).HasConversion<string>();
            entity.Property(user => user.Status).HasConversion<string>();
        });

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.HasIndex(item => item.ExpiresAt);
            entity.HasIndex(item => new { item.Email, item.Purpose });
            entity.Property(item => item.Purpose)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.HasOne(item => item.User)
                .WithMany(user => user.EmailVerifications)
                .HasForeignKey(item => item.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Novel>(entity =>
        {
            entity.HasIndex(novel => novel.Title);
            entity.HasIndex(novel => novel.AuthorId);
            entity.Property(novel => novel.Synopsis).HasColumnType("nvarchar(max)");
            entity.Property(novel => novel.Note).HasColumnType("nvarchar(max)");
            entity.Property(novel => novel.IsActive).HasDefaultValue(true);
            entity.Property(novel => novel.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasOne(novel => novel.Author)
                .WithMany(user => user.AuthoredNovels)
                .HasForeignKey(novel => novel.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasIndex(chapter => new { chapter.NovelId, chapter.ChapterNumber })
                .IsUnique();
            entity.Property(chapter => chapter.Content).HasColumnType("nvarchar(max)");

            entity.HasOne(chapter => chapter.Novel)
                .WithMany(novel => novel.Chapters)
                .HasForeignKey(chapter => chapter.NovelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(category => category.Name).IsUnique();
        });

        modelBuilder.Entity<NovelCategory>(entity =>
        {
            entity.HasKey(item => new { item.NovelId, item.CategoryId });

            entity.HasOne(item => item.Novel)
                .WithMany(novel => novel.NovelCategories)
                .HasForeignKey(item => item.NovelId);

            entity.HasOne(item => item.Category)
                .WithMany(category => category.NovelCategories)
                .HasForeignKey(item => item.CategoryId);
        });

        modelBuilder.Entity<ReadingHistory>(entity =>
        {
            entity.HasIndex(history => new { history.UserId, history.NovelId })
                .IsUnique();
            entity.HasIndex(history => history.LastReadAt);

            entity.HasOne(history => history.User)
                .WithMany(user => user.ReadingHistories)
                .HasForeignKey(history => history.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(history => history.Novel)
                .WithMany(novel => novel.ReadingHistories)
                .HasForeignKey(history => history.NovelId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(history => history.Chapter)
                .WithMany(chapter => chapter.ReadingHistories)
                .HasForeignKey(history => history.ChapterId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(follow => new { follow.UserId, follow.NovelId });
            entity.HasIndex(follow => follow.CreatedAt);

            entity.HasOne(follow => follow.User)
                .WithMany(user => user.FollowedNovels)
                .HasForeignKey(follow => follow.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(follow => follow.Novel)
                .WithMany(novel => novel.Followers)
                .HasForeignKey(follow => follow.NovelId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ChapterComment>(entity =>
        {
            entity.HasIndex(comment => comment.ChapterId);
            entity.HasIndex(comment => comment.UserId);

            entity.HasOne(comment => comment.Chapter)
                .WithMany(chapter => chapter.Comments)
                .HasForeignKey(comment => comment.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(comment => comment.User)
                .WithMany(user => user.ChapterComments)
                .HasForeignKey(comment => comment.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(comment => comment.ParentComment)
                .WithMany(comment => comment.Replies)
                .HasForeignKey(comment => comment.ParentCommentId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<NovelComment>(entity =>
        {
            entity.HasIndex(comment => comment.NovelId);
            entity.HasIndex(comment => comment.UserId);

            entity.HasOne(comment => comment.Novel)
                .WithMany(novel => novel.Comments)
                .HasForeignKey(comment => comment.NovelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(comment => comment.User)
                .WithMany(user => user.NovelComments)
                .HasForeignKey(comment => comment.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(comment => comment.ParentComment)
                .WithMany(comment => comment.Replies)
                .HasForeignKey(comment => comment.ParentCommentId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
