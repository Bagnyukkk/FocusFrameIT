using FocusFrameAPI.Controllers;
using FocusFrameAPI.Dtos.Homework;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameAPI.Services.Interfaces;
using FocusFrameTests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FocusFrameTests.Integration
{
  public class HomeworksControllerTests : IntegrationTestBase
  {
    [Fact]
    public async Task SubmitHomework_ValidFile_ReturnsCreated()
    {
      // Arrange
      // Seed DB
      var user = new User { Id = 1, Email = "u", PasswordHash = "x", Role = UserRole.Student, FullName = "U" };
      var lesson = new Lesson { Id = 5, Title = "L", CourseId = 1, Course = new Course { Title = "C" } };
      Context.Users.Add(user);
      Context.Lessons.Add(lesson);
      await Context.SaveChangesAsync();

      // Mock Dependencies
      var mockNotify = new Mock<INotificationService>();
      var mockEnv = new Mock<IWebHostEnvironment>();
      mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

      var service = new HomeworkService(Context, mockNotify.Object, mockEnv.Object); // [cite: 112]
      var controller = new HomeworksController(service);
      SetupUserContext(controller, 1, UserRole.Student);

      // Mock File
      var fileMock = new Mock<IFormFile>();
      var ms = new MemoryStream();
      fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
      fileMock.Setup(f => f.FileName).Returns("hw.pdf");
      fileMock.Setup(f => f.Length).Returns(100);

      // Act
      var result = await controller.SubmitHomework(5, fileMock.Object);

      // Assert
      var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result); // [cite: 382]
      var dto = Assert.IsType<HomeworkDto>(createdResult.Value);
      Assert.Equal(HomeworkStatus.Pending, dto.Status);
    }

    [Fact]
    public async Task GradeHomework_AdminRejects_ReturnsOk()
    {
      // Arrange
      var submission = new HomeworkSubmission { Id = 10, StudentId = 1, LessonId = 1, Status = HomeworkStatus.Pending };
      Context.Users.Add(new User { Id = 1, Email = "u", PasswordHash = "x", FullName = "U" });
      Context.Lessons.Add(new Lesson { Id = 1, Title = "L", Course = new Course { Title = "C" } });
      Context.HomeworkSubmissions.Add(submission);
      await Context.SaveChangesAsync();

      var service = new HomeworkService(Context, new Mock<INotificationService>().Object, new Mock<IWebHostEnvironment>().Object);
      var controller = new HomeworksController(service);
      SetupUserContext(controller, 2, UserRole.Admin);

      var dto = new GradeHomeworkDto { Status = HomeworkStatus.Rejected, AdminComment = "Redo" };

      // Act
      var result = await controller.GradeHomework(10, dto);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result); // [cite: 386]
      var hw = Assert.IsType<HomeworkDto>(okResult.Value);
      Assert.Equal(HomeworkStatus.Rejected, hw.Status);
    }
  }
}
