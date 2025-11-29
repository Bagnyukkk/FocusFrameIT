using FocusFrameAPI.Data;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameTests
{
  public class AdminDashboardServiceTests
  {
    private FocusFrameDbContext GetInMemoryDbContext()
    {
      var options = new DbContextOptionsBuilder<FocusFrameDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
      return new FocusFrameDbContext(options);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_ShouldCountCorrectly()
    {
      // Arrange
      using var context = GetInMemoryDbContext();
      var service = new AdminDashboardService(context);

      // Seed Users
      context.Users.AddRange(
          new User { Id = 1, Email = "u1", PasswordHash = "x", Role = UserRole.Student },
          new User { Id = 2, Email = "u2", PasswordHash = "x", Role = UserRole.Admin }
      );

      // Seed Courses
      context.Courses.Add(new Course { Id = 1, Title = "C1" });

      // Seed Homework (Only 2 are Pending)
      var lesson = new Lesson { Id = 1, Title = "L1", CourseId = 1 }; // Requires existing foreign key usually, but EF InMemory is lenient
      context.Lessons.Add(lesson);

      context.HomeworkSubmissions.AddRange(
          new HomeworkSubmission { Id = 1, Status = HomeworkStatus.Pending, LessonId = 1, StudentId = 1 },
          new HomeworkSubmission { Id = 2, Status = HomeworkStatus.Pending, LessonId = 1, StudentId = 1 },
          new HomeworkSubmission { Id = 3, Status = HomeworkStatus.Approved, LessonId = 1, StudentId = 1 },
          new HomeworkSubmission { Id = 4, Status = HomeworkStatus.Rejected, LessonId = 1, StudentId = 1 }
      );

      await context.SaveChangesAsync();

      // Act
      var stats = await service.GetDashboardStatsAsync();

      // Assert
      Assert.Equal(2, stats.TotalUsers); // Total users in DB
      Assert.Equal(1, stats.TotalCourses);
      Assert.Equal(2, stats.PendingHomeworkCount); // Only the Pending ones
    }

    [Fact]
    public async Task GetStudentsWithProgressAsync_ShouldFilterByRole_AndCalcProgress()
    {
      // Arrange
      using var context = GetInMemoryDbContext();
      var service = new AdminDashboardService(context);

      // 1. Create Users
      var student = new User { Id = 1, FullName = "John Doe", Email = "john@test.com", Role = UserRole.Student, PasswordHash = "x" };
      var admin = new User { Id = 2, FullName = "Admin User", Email = "admin@test.com", Role = UserRole.Admin, PasswordHash = "x" };

      context.Users.AddRange(student, admin);

      // 2. Create Course and Lessons
      var course = new Course { Id = 10, Title = "Photo Basics" };
      var lesson1 = new Lesson { Id = 101, Title = "Intro", CourseId = 10 };
      var lesson2 = new Lesson { Id = 102, Title = "Camera", CourseId = 10 };
      context.Courses.Add(course);
      context.Lessons.AddRange(lesson1, lesson2);

      // 3. Create Enrollment
      var enrollment = new Enrollment
      {
        Id = 50,
        UserId = student.Id,
        CourseId = course.Id,
        Status = EnrollmentStatus.InProgress
      };
      context.Enrollments.Add(enrollment);

      // 4. Create Progress (1 completed, 1 incomplete)
      context.LessonProgresses.AddRange(
          new LessonProgress { EnrollmentId = 50, LessonId = 101, IsCompleted = true },
          new LessonProgress { EnrollmentId = 50, LessonId = 102, IsCompleted = false }
      );

      await context.SaveChangesAsync();

      // Act
      var results = await service.GetStudentsWithProgressAsync();

      // Assert
      Assert.Single(results); // Should not contain Admin
      var studentResult = results.First();

      Assert.Equal("John Doe", studentResult.FullName);
      Assert.Equal(1, studentResult.CoursesEnrolled);

      // Logic Check: 2 total progress entries, 1 completed = 50%
      Assert.Equal(50.0, studentResult.OverallProgressPercent);
    }

    [Fact]
    public async Task GetStudentsWithProgressAsync_ShouldHandleZeroProgress_AvoidDivideByZero()
    {
      // Arrange
      using var context = GetInMemoryDbContext();
      var service = new AdminDashboardService(context);

      var student = new User { Id = 1, Email = "new@test.com", Role = UserRole.Student, PasswordHash = "x" };
      context.Users.Add(student);

      // Enrollment exists, but no LessonProgress records created yet
      var enrollment = new Enrollment { Id = 1, UserId = 1, CourseId = 1 };
      context.Enrollments.Add(enrollment);

      await context.SaveChangesAsync();

      // Act
      var results = await service.GetStudentsWithProgressAsync();

      // Assert
      var studentResult = results.First();
      Assert.Equal(0, studentResult.OverallProgressPercent);
    }
  }
}
