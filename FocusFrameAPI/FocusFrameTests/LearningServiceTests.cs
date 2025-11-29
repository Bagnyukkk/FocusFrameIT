using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Lesson;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameAPI.Services.Interfaces;
using FocusFrameTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FocusFrameTests
{
  public class LearningServiceTests
  {
    private readonly Mock<INotificationService> _notificationMock;

    public LearningServiceTests()
    {
      _notificationMock = new Mock<INotificationService>();
    }

    [Fact]
    public async Task JoinCourseAsync_NewEnrollment_AddsToDatabase()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var service = new LearningService(context, _notificationMock.Object);

      int userId = 1;
      int courseId = 10;

      // Act
      await service.JoinCourseAsync(userId, courseId);

      // Assert
      var enrollment = await context.Enrollments
          .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

      Assert.NotNull(enrollment);
      Assert.Equal(EnrollmentStatus.InProgress, enrollment.Status); // [cite: 47]
    }

    [Fact]
    public async Task SubmitQuizAsync_ScoreBelow70_DoesNotPassQuiz()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var enrollment = SetupEnrollmentWithLesson(context);
      var service = new LearningService(context, _notificationMock.Object);

      var submission = new QuizSubmissionDto { ScorePercent = 60 }; // Below 70% threshold [cite: 219]

      // Act
      await service.SubmitQuizAsync(enrollment.UserId, enrollment.Course.Lessons.First().Id, submission);

      // Assert
      var progress = await context.LessonProgresses.FirstAsync();
      Assert.Equal(60, progress.QuizScorePercent);
      Assert.False(progress.QuizPassed); // Should be false
    }

    [Fact]
    public async Task SubmitQuizAsync_ScoreAbove70_PassesQuiz_And_IssuesCertificate()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var enrollment = SetupEnrollmentWithLesson(context);
      var service = new LearningService(context, _notificationMock.Object);

      var submission = new QuizSubmissionDto { ScorePercent = 80 };

      // Act
      await service.SubmitQuizAsync(enrollment.UserId, enrollment.Course.Lessons.First().Id, submission);

      // Assert 1 & 2: Database State
      var updatedEnrollment = await context.Enrollments.FindAsync(enrollment.Id);
      Assert.True(updatedEnrollment.IsCertificateIssued);

      // Assert 3: Notification Sent
      // Fix: Match "certificate" (lowercase) OR check the Title parameter which is "Certificate Ready!"
      _notificationMock.Verify(n => n.CreateAndSendAsync(
          enrollment.UserId,
          It.Is<string>(t => t == "Certificate Ready!"), // Check Title instead (Specific)
          It.Is<string>(m => m.Contains("certificate")), // Check Message (Case corrected) [cite: 243]
          NotificationType.System,
          enrollment.CourseId
      ), Times.Once);
    }

    // Helper to seed data for LearningService tests
    private Enrollment SetupEnrollmentWithLesson(FocusFrameDbContext context)
    {
      var user = new User { Email = "learner@test.com", PasswordHash = "x", FullName = "Learner" };
      var course = new Course { Title = "Quiz Course" };
      var lesson = new Lesson { Title = "Final Exam", Course = course, OrderIndex = 1 };

      context.Users.Add(user);
      context.Courses.Add(course);
      context.Lessons.Add(lesson);

      var enrollment = new Enrollment
      {
        User = user,
        Course = course,
        Status = EnrollmentStatus.InProgress
      };

      context.Enrollments.Add(enrollment);
      context.SaveChanges();

      return enrollment;
    }
  }
}
