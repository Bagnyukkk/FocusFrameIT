using FocusFrameAPI.Controllers;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameTests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FocusFrameTests.Integration
{
  public class CoursesControllerTests : IntegrationTestBase
  {
    [Fact]
    public async Task GetLessonContent_NotEnrolled_ReturnsForbidden()
    {
      // Arrange
      // Seed Data
      var user = new User { Id = 1, Email = "u", PasswordHash = "x", Role = UserRole.Student, FullName = "U" };
      var course = new Course { Id = 10, Title = "C" };
      var lesson = new Lesson { Id = 101, Title = "L", CourseId = 10, VideoUrl = "vid" };
      Context.Users.Add(user);
      Context.Courses.Add(course);
      Context.Lessons.Add(lesson);
      await Context.SaveChangesAsync();

      var service = new CourseService(Context); // [cite: 60]
      var controller = new CoursesController(service);
      SetupUserContext(controller, user.Id, UserRole.Student);

      // Act
      var result = await controller.GetLessonContent(101);

      // Assert
      // Service returns null if not enrolled [cite: 68], Controller returns 403 [cite: 355]
      var objectResult = Assert.IsType<ObjectResult>(result);
      Assert.Equal(403, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetLessonContent_Enrolled_ReturnsContent()
    {
      // Arrange
      var user = new User { Id = 1, Email = "u", PasswordHash = "x", Role = UserRole.Student, FullName = "U" };
      var course = new Course { Id = 10, Title = "C" };
      var lesson = new Lesson { Id = 101, Title = "L", CourseId = 10, VideoUrl = "vid" };
      Context.Users.Add(user);
      Context.Courses.Add(course);
      Context.Lessons.Add(lesson);
      Context.Enrollments.Add(new Enrollment { UserId = 1, CourseId = 10, Status = EnrollmentStatus.InProgress });
      await Context.SaveChangesAsync();

      var service = new CourseService(Context);
      var controller = new CoursesController(service);
      SetupUserContext(controller, 1, UserRole.Student);

      // Act
      var result = await controller.GetLessonContent(101);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result); // [cite: 356]
      var content = Assert.IsType<FocusFrameAPI.Dtos.Lesson.LessonContentDto>(okResult.Value);
      Assert.Equal("vid", content.VideoUrl);
    }
  }
}
