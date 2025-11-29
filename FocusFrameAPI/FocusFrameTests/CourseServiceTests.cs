using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameTests.Helpers;

namespace FocusFrameTests
{
  public class CourseServiceTests
  {
    [Fact]
    public async Task GetAllCoursesAsync_ReturnsAllCourses_MappedCorrectly()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      context.Courses.AddRange(
          new Course { Title = "C# Basics", Description = "Intro", MentorName = "Alice", Difficulty = "Easy" },
          new Course { Title = "Advanced .NET", Description = "Deep dive", MentorName = "Bob", Difficulty = "Hard" }
      );
      await context.SaveChangesAsync();

      var service = new CourseService(context);

      // Act
      var result = await service.GetAllCoursesAsync();

      // Assert
      Assert.Equal(2, result.Count());
      Assert.Contains(result, c => c.Title == "C# Basics");
      Assert.Contains(result, c => c.MentorName == "Bob");
    }

    [Fact]
    public async Task GetLessonContentAsync_UserNotEnrolled_ReturnsNull()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var user = new User { Email = "test@test.com", PasswordHash = "hash", FullName = "Test" };
      var course = new Course { Title = "Test Course" };
      var lesson = new Lesson
      {
        Title = "Secret Lesson",
        Course = course,
        VideoUrl = "http://secret-video.com",
        TextContent = "Secret Text"
      };

      context.Users.Add(user);
      context.Courses.Add(course);
      context.Lessons.Add(lesson);
      await context.SaveChangesAsync();

      var service = new CourseService(context);

      // Act
      // Attempt to access content without an Enrollment record
      var result = await service.GetLessonContentAsync(user.Id, lesson.Id);

      // Assert
      Assert.Null(result); // Should be null because user is not enrolled [cite: 13, 63]
    }

    [Fact]
    public async Task GetLessonContentAsync_UserEnrolled_ReturnsContent()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var user = new User { Email = "student@test.com", PasswordHash = "hash", FullName = "Student" };
      var course = new Course { Title = "Test Course" };
      var lesson = new Lesson
      {
        Title = "Lesson 1",
        Course = course,
        VideoUrl = "http://video.mp4",
        TextContent = "Content"
      };

      context.Users.Add(user);
      context.Courses.Add(course);
      context.Lessons.Add(lesson);

      // Create Enrollment
      context.Enrollments.Add(new Enrollment
      {
        User = user,
        Course = course,
        Status = EnrollmentStatus.InProgress
      });

      await context.SaveChangesAsync();

      var service = new CourseService(context);

      // Act
      var result = await service.GetLessonContentAsync(user.Id, lesson.Id);

      // Assert
      Assert.NotNull(result);
      Assert.Equal("http://video.mp4", result.VideoUrl); // Access granted
    }
  }
}
