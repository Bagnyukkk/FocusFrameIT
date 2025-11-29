using FocusFrameAPI.Controllers;
using FocusFrameAPI.Dtos.Chat;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameAPI.Services.Interfaces;
using FocusFrameTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FocusFrameTests.Integration
{
  public class ChatControllerTests : IntegrationTestBase
  {
    [Fact]
    public async Task SendMessage_StudentSends_CreatesMessageAndReturnsOk()
    {
      // Arrange
      var mockNotify = new Mock<INotificationService>();
      var service = new ChatService(Context, mockNotify.Object); // [cite: 29]
      var controller = new ChatController(service);

      var studentId = 10;
      SetupUserContext(controller, studentId, UserRole.Student);

      // Act
      var dto = new SendMessageDto { MessageText = "Help me" };
      var result = await controller.SendMessage(dto);

      // Assert
      Assert.IsType<OkResult>(result);

      // Verify DB Side Effect
      var msg = Context.ChatMessages.FirstOrDefault();
      Assert.NotNull(msg);
      Assert.Equal("Help me", msg.MessageText);
      Assert.Equal(studentId, msg.SenderId);
    }

    [Fact]
    public async Task GetHistory_AdminAccessingWithoutStudentId_ReturnsBadRequest()
    {
      // Arrange
      var service = new ChatService(Context, new Mock<INotificationService>().Object);
      var controller = new ChatController(service);

      SetupUserContext(controller, 99, UserRole.Admin); // Role Admin [cite: 345]

      // Act
      var result = await controller.GetHistory(null);

      // Assert
      Assert.IsType<BadRequestObjectResult>(result); // [cite: 345]
    }
  }
}
