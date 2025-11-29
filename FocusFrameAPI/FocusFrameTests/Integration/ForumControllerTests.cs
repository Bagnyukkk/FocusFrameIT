using FocusFrameAPI.Controllers;
using FocusFrameAPI.Dtos.Forum;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameAPI.Services.Interfaces;
using FocusFrameTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FocusFrameTests.Integration
{
  public class ForumControllerTests : IntegrationTestBase
  {
    [Fact]
    public async Task CreateTopic_ValidData_ReturnsCreated()
    {
      // Arrange
      Context.Users.Add(new User { Id = 1, Email = "x", PasswordHash = "x", FullName = "x" });
      await Context.SaveChangesAsync();

      var mockNotify = new Mock<INotificationService>();
      var service = new ForumService(Context, mockNotify.Object);
      var controller = new ForumController(service);
      SetupUserContext(controller, 1, UserRole.Student);

      var dto = new CreateTopicDto { Title = "New Topic", Content = "Content" };

      // Act
      var result = await controller.CreateTopic(dto);

      // Assert
      var createdResult = Assert.IsType<CreatedAtActionResult>(result); // [cite: 372]
      var returnDto = Assert.IsType<TopicDto>(createdResult.Value);
      Assert.Equal("New Topic", returnDto.Title);
      Assert.Single(Context.ForumTopics);
    }

    [Fact]
    public async Task DeleteTopic_Admin_CanDelete()
    {
      // Arrange
      Context.Users.Add(new User { Id = 1, Email = "x", PasswordHash = "x", FullName = "x" });
      Context.ForumTopics.Add(new ForumTopic { Id = 10, Title = "T", AuthorId = 1 });
      await Context.SaveChangesAsync();

      var service = new ForumService(Context, new Mock<INotificationService>().Object);
      var controller = new ForumController(service);
      SetupUserContext(controller, 99, UserRole.Admin); // Admin ID 99

      // Act
      var result = await controller.DeleteTopic(10);

      // Assert
      Assert.IsType<NoContentResult>(result); // [cite: 375]
      Assert.Empty(Context.ForumTopics);
    }
  }
}
