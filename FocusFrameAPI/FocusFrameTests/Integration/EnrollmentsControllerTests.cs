using FocusFrameAPI.Controllers;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameAPI.Services.Interfaces;
using FocusFrameTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FocusFrameTests.Integration
{
  public class EnrollmentsControllerTests : IntegrationTestBase
  {
    [Fact]
    public async Task JoinCourse_ValidRequest_CreatesEnrollment()
    {
      // Arrange
      var user = new User { Id = 1, Email = "u", PasswordHash = "x", FullName = "u" };
      var course = new Course { Id = 10, Title = "C" };
      Context.Users.Add(user);
      Context.Courses.Add(course);
      await Context.SaveChangesAsync();

      var mockNotify = new Mock<INotificationService>();
      var service = new LearningService(Context, mockNotify.Object); // [cite: 145]
      var controller = new EnrollmentsController(service);
      SetupUserContext(controller, 1, UserRole.Student);

      // Act
      var result = await controller.JoinCourse(10);

      // Assert
      Assert.IsType<OkObjectResult>(result);
      Assert.NotNull(Context.Enrollments.FirstOrDefault(e => e.UserId == 1 && e.CourseId == 10)); // [cite: 147]
        }
  }
}
