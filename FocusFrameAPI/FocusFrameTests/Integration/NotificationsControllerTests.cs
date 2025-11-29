using FocusFrameAPI.Controllers;
using FocusFrameAPI.Dtos.Admin;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Services;
using FocusFrameAPI.Services.Interfaces;
using FocusFrameTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace FocusFrameTests.Integration
{
  public class NotificationsControllerTests : IntegrationTestBase
  {
    [Fact]
    public async Task GetUnreadNotifications_ReturnsList()
    {
      // Arrange
      Context.Users.Add(new User { Id = 1, Email = "u", PasswordHash = "x", FullName = "u" });
      Context.Notifications.Add(new Notification { UserId = 1, Title = "N1", IsRead = false });
      await Context.SaveChangesAsync();

      // Mock SignalR
      var mockHub = new Mock<IHubContext<NotificationHub, INotificationClient>>();
      var service = new NotificationService(Context, mockHub.Object);

      var controller = new NotificationsController(service);
      SetupUserContext(controller, 1, FocusFrameAPI.Enums.UserRole.Student);

      // Act
      var result = await controller.GetUnreadNotifications();

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result);

      // FIX: Assert against NotificationDto, not Notification entity
      // The error explicitly stated the actual type is List<NotificationDto>
      var list = Assert.IsAssignableFrom<IEnumerable<NotificationDto>>(okResult.Value);

      Assert.Single(list);
      Assert.Equal("N1", list.First().Title); // Optional: Verify data mapping
    }
  }
}
