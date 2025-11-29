using FocusFrameAPI.Controllers;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameTests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FocusFrameTests.Integration
{
  public class AdminControllerTests : IntegrationTestBase
  {
    [Fact]
    public async Task GetDashboardStats_ReturnsCorrectStats()
    {
      // Arrange
      // Seed DB
      Context.Users.AddRange(
          new User { Id = 1, Role = UserRole.Student, Email = "s@t.com", PasswordHash = "x", FullName = "S" },
          new User { Id = 2, Role = UserRole.Admin, Email = "a@t.com", PasswordHash = "x", FullName = "A" }
      );
      Context.Courses.Add(new Course { Id = 1, Title = "C1" });
      Context.HomeworkSubmissions.Add(new HomeworkSubmission
      {
        Id = 1,
        Status = HomeworkStatus.Pending,
        LessonId = 1,
        StudentId = 1,
        Lesson = new Lesson { Id = 1, Title = "L", CourseId = 1 }
      });
      await Context.SaveChangesAsync();

      var service = new AdminDashboardService(Context); // [cite: 4]
      var controller = new AdminController(service);
      SetupUserContext(controller, 2, UserRole.Admin); // Admin User

      // Act
      var result = await controller.GetDashboardStats();

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var stats = Assert.IsType<FocusFrameAPI.Dtos.Admin.AdminStatsDto>(okResult.Value);

      Assert.Equal(2, stats.TotalUsers); // [cite: 10]
      Assert.Equal(1, stats.TotalCourses);
      Assert.Equal(1, stats.PendingHomeworkCount);
    }
  }
}
