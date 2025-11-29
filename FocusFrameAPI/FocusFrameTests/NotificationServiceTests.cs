using FocusFrameAPI.Data;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FocusFrameTests
{
  public class NotificationServiceTests
  {
    private readonly Mock<IHubContext<NotificationHub, INotificationClient>> _mockHubContext;
    private readonly Mock<IHubClients<INotificationClient>> _mockClients;
    private readonly Mock<INotificationClient> _mockClientProxy;

    public NotificationServiceTests()
    {
      // Setup SignalR Mocks
      _mockHubContext = new Mock<IHubContext<NotificationHub, INotificationClient>>();
      _mockClients = new Mock<IHubClients<INotificationClient>>();
      _mockClientProxy = new Mock<INotificationClient>();

      _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);
      _mockClients.Setup(c => c.User(It.IsAny<string>())).Returns(_mockClientProxy.Object);
    }

    // Helper to get a fresh In-Memory DbContext for each test
    private FocusFrameDbContext GetInMemoryDbContext()
    {
      var options = new DbContextOptionsBuilder<FocusFrameDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name per call
          .Options;
      return new FocusFrameDbContext(options);
    }

    [Fact]
    public async Task CreateAndSendAsync_ShouldSaveToDb_AndSendViaSignalR()
    {
      // Arrange
      using var context = GetInMemoryDbContext();
      var service = new NotificationService(context, _mockHubContext.Object);
      int userId = 1;

      // Act
      await service.CreateAndSendAsync(userId, "Test Title", "Test Message", NotificationType.System);

      // Assert - Database
      var notification = await context.Notifications.FirstOrDefaultAsync();
      Assert.NotNull(notification);
      Assert.Equal("Test Title", notification.Title);
      Assert.Equal(userId, notification.UserId);
      Assert.False(notification.IsRead);

      // Assert - SignalR
      // Verify that ReceiveNotification was called exactly once on the specific User client
      _mockClients.Verify(clients => clients.User(userId.ToString()), Times.Once);
      _mockClientProxy.Verify(client => client.ReceiveNotification(It.Is<Notification>(n =>
          n.Title == "Test Title" && n.UserId == userId)), Times.Once);
    }

    [Fact]
    public async Task GetUnreadAsync_ShouldReturnOnlyUnread_ForSpecificUser()
    {
      // Arrange
      using var context = GetInMemoryDbContext();
      var service = new NotificationService(context, _mockHubContext.Object);
      var user1 = new User { Id = 1, Email = "test@test.com", PasswordHash = "hash", Role = UserRole.Student };

      context.Users.Add(user1);
      context.Notifications.AddRange(
          new Notification { UserId = 1, Title = "Unread 1", IsRead = false, Message = "Msg" },
          new Notification { UserId = 1, Title = "Read 1", IsRead = true, Message = "Msg" },
          new Notification { UserId = 2, Title = "Other User Unread", IsRead = false, Message = "Msg" }
      );
      await context.SaveChangesAsync();

      // Act
      var result = await service.GetUnreadAsync(1);

      // Assert
      Assert.Single(result); // Should only find 1
      Assert.Equal("Unread 1", result.First().Title);
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldUpdateStatus_AndReadAtTimestamp()
    {
      // Arrange
      using var context = GetInMemoryDbContext();
      var service = new NotificationService(context, _mockHubContext.Object);

      var noteId = 10;
      var userId = 5;

      var notification = new Notification
      {
        Id = noteId,
        UserId = userId,
        IsRead = false,
        Title = "Test",
        Message = "Msg"
      };

      context.Notifications.Add(notification);
      await context.SaveChangesAsync();

      // Act
      await service.MarkAsReadAsync(noteId, userId);

      // Assert
      var dbNote = await context.Notifications.FindAsync(noteId);
      Assert.True(dbNote!.IsRead);
      Assert.NotNull(dbNote.ReadAt);
    }

    [Fact]
    public async Task MarkAllAsReadAsync_ShouldUpdateAllUnread_ForUser()
    {
      // Arrange
      using var context = GetInMemoryDbContext();
      var service = new NotificationService(context, _mockHubContext.Object);
      int userId = 1;

      context.Notifications.AddRange(
          new Notification { UserId = userId, IsRead = false, Title = "1", Message = "m" },
          new Notification { UserId = userId, IsRead = false, Title = "2", Message = "m" },
          new Notification { UserId = userId, IsRead = true, Title = "3", Message = "m" } // Already read
      );
      await context.SaveChangesAsync();

      // Act
      await service.MarkAllAsReadAsync(userId);

      // Assert
      var unreadCount = await context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
      Assert.Equal(0, unreadCount);
    }
  }
}
