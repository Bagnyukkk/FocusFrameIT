using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Lesson;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameAPI.Services
{
  public class LearningService : ILearningService
  {
    private readonly FocusFrameDbContext _context;
    private readonly INotificationService _notificationService;

    public LearningService(FocusFrameDbContext context, INotificationService notificationService)
    {
      _context = context;
      _notificationService = notificationService;
        }

    public async Task JoinCourseAsync(int userId, int courseId)
    {
      // Check if already enrolled
      bool exists = await _context.Enrollments
          .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);

      if (exists) return;

      var enrollment = new Enrollment
      {
        UserId = userId,
        CourseId = courseId,
        Status = EnrollmentStatus.InProgress,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      _context.Enrollments.Add(enrollment);
      await _context.SaveChangesAsync();
    }

    public async Task MarkLessonViewedAsync(int userId, int lessonId)
    {
      var lesson = await _context.Lessons.FindAsync(lessonId);
      if (lesson == null) return;

      var enrollment = await _context.Enrollments
          .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == lesson.CourseId);

      if (enrollment == null) return; // User not enrolled

      // Find or Create Progress record
      var progress = await _context.LessonProgresses
          .FirstOrDefaultAsync(lp => lp.EnrollmentId == enrollment.Id && lp.LessonId == lessonId);

      if (progress == null)
      {
        progress = new LessonProgress
        {
          EnrollmentId = enrollment.Id,
          LessonId = lessonId,
          IsCompleted = true,
          CompletedAt = DateTime.UtcNow
        };
        _context.LessonProgresses.Add(progress);
      }
      else if (!progress.IsCompleted)
      {
        progress.IsCompleted = true;
        progress.CompletedAt = DateTime.UtcNow;
      }

      await _context.SaveChangesAsync();

      // Check if this action completed the course
      await CheckAndIssueCertificateAsync(enrollment.Id);
    }

    public async Task SubmitQuizAsync(int userId, int lessonId, QuizSubmissionDto submission)
    {
      var lesson = await _context.Lessons.FindAsync(lessonId);
      if (lesson == null) return;

      var enrollment = await _context.Enrollments
          .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == lesson.CourseId);
      if (enrollment == null) return;

      var progress = await _context.LessonProgresses
          .FirstOrDefaultAsync(lp => lp.EnrollmentId == enrollment.Id && lp.LessonId == lessonId);

      if (progress == null)
      {
        progress = new LessonProgress { EnrollmentId = enrollment.Id, LessonId = lessonId };
        _context.LessonProgresses.Add(progress);
      }

      // Update Quiz stats
      progress.QuizScorePercent = submission.ScorePercent;
      progress.QuizPassed = submission.ScorePercent >= 70; // 70% threshold
      progress.IsCompleted = true; // Passing quiz implies lesson completion
      progress.CompletedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      // Check Certificate Logic
      await CheckAndIssueCertificateAsync(enrollment.Id);
    }

    private async Task CheckAndIssueCertificateAsync(int enrollmentId)
    {
      var enrollment = await _context.Enrollments
          .Include(e => e.Course)
          .ThenInclude(c => c.Lessons)
          .Include(e => e.LessonProgresses)
          .FirstOrDefaultAsync(e => e.Id == enrollmentId);

      if (enrollment == null || enrollment.IsCertificateIssued) return;

      var totalLessons = enrollment.Course.Lessons.Count;
      // Check if all lessons have a corresponding Completed progress record
      var completedCount = enrollment.LessonProgresses.Count(lp => lp.IsCompleted);

      // Logic: All lessons must be completed. 
      // Note: If the final lesson requires a quiz, logic above ensures IsCompleted is only true if passed.
      if (completedCount >= totalLessons && totalLessons > 0)
      {
        // Issue Certificate
        enrollment.IsCertificateIssued = true;
        enrollment.Status = EnrollmentStatus.Completed;
        enrollment.CompletionDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Trigger Notification Service
        await _notificationService.CreateAndSendAsync(
            userId: enrollment.UserId,
            title: "Certificate Ready!",
            message: $"Congratulations! You have completed {enrollment.Course.Title}. Your certificate is ready to download.",
            type: NotificationType.System,
            referenceId: enrollment.CourseId
        );
      }
    }
  }
}
