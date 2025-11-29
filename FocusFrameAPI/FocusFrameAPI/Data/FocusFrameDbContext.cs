using FocusFrameAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameAPI.Data
{
  public class FocusFrameDbContext : DbContext
  {
    public FocusFrameDbContext(DbContextOptions<FocusFrameDbContext> options)
            : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }
    public DbSet<HomeworkSubmission> HomeworkSubmissions { get; set; }
    public DbSet<ChatThread> ChatThreads { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ForumTopic> ForumTopics { get; set; }
    public DbSet<ForumComment> ForumComments { get; set; }
    public DbSet<PortfolioAlbum> PortfolioAlbums { get; set; }
    public DbSet<PortfolioPhoto> PortfolioPhotos { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>(entity =>
      {
        entity.HasIndex(e => e.Email).IsUnique();

        entity.Property(e => e.Role)
              .HasConversion<string>();
      });

      modelBuilder.Entity<Notification>(entity =>
      {
        entity.HasOne(n => n.User)
              .WithMany()
              .HasForeignKey(n => n.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.Property(e => e.Type)
              .HasConversion<string>();
      });

      modelBuilder.Entity<Course>(entity =>
      {
        entity.HasMany(c => c.Lessons)
              .WithOne(l => l.Course)
              .HasForeignKey(l => l.CourseId)
              .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<Lesson>(entity =>
      {
        entity.HasMany(l => l.HomeworkSubmissions)
              .WithOne(h => h.Lesson)
              .HasForeignKey(h => h.LessonId)
              .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<Enrollment>(entity =>
      {
        entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();

        entity.Property(e => e.Status)
              .HasConversion<string>();

        entity.HasMany(e => e.LessonProgresses)
              .WithOne(lp => lp.Enrollment)
              .HasForeignKey(lp => lp.EnrollmentId)
              .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<LessonProgress>(entity =>
      {
        entity.HasIndex(lp => new { lp.EnrollmentId, lp.LessonId }).IsUnique();
      });

      modelBuilder.Entity<HomeworkSubmission>(entity =>
      {
        entity.Property(h => h.Status)
              .HasConversion<string>();
      });

      modelBuilder.Entity<ChatThread>(entity =>
      {
        entity.HasMany(t => t.Messages)
              .WithOne(m => m.Thread)
              .HasForeignKey(m => m.ThreadId)
              .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<ChatThread>()
                .HasIndex(t => t.UserId)
                .IsUnique();

      modelBuilder.Entity<ChatMessage>(entity =>
      {
        entity.HasOne(m => m.Sender)
              .WithMany(u => u.SentMessages)
              .HasForeignKey(m => m.SenderId)
              .OnDelete(DeleteBehavior.Restrict);
      });

      modelBuilder.Entity<User>()
                .HasMany(u => u.Enrollments)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<User>()
          .HasMany(u => u.HomeworkSubmissions)
          .WithOne(h => h.Student)
          .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<User>()
          .HasMany(u => u.PortfolioAlbums)
          .WithOne(a => a.User)
          .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<PortfolioAlbum>(entity =>
      {
        entity.HasMany(a => a.Photos)
              .WithOne(p => p.Album)
              .HasForeignKey(p => p.AlbumId)
              .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<ForumTopic>(entity =>
      {
        entity.HasMany(t => t.Comments)
              .WithOne(c => c.Topic)
              .HasForeignKey(c => c.TopicId)
              .OnDelete(DeleteBehavior.Cascade);
      });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      var entries = ChangeTracker
          .Entries()
          .Where(e => e.Entity is BaseEntity && (
                  e.State == EntityState.Added
                  || e.State == EntityState.Modified));

      foreach (var entityEntry in entries)
      {
        ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

        if (entityEntry.State == EntityState.Added)
        {
          ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
        }
      }

      return base.SaveChangesAsync(cancellationToken);
    }
  }
}