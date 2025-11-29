using FocusFrameAPI.Dtos.Chat;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameAPI.Services.Interfaces;
using FocusFrameTests.Helpers;
using Moq;

namespace FocusFrameTests
{
  public class ChatServiceTests
  {
    [Fact]
    public async Task SendMessageAsync_Student_CreatesNewThread_IfNoneExists()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var mockNotification = new Mock<INotificationService>();
      var service = new ChatService(context, mockNotification.Object);

      // Act
      var dto = new SendMessageDto { MessageText = "Hello Admin" };
      await service.SendMessageAsync(100, UserRole.Student, dto);

      // Assert
      var thread = context.ChatThreads.FirstOrDefault(t => t.UserId == 100);
      Assert.NotNull(thread);
      Assert.True(thread.HasUnreadForAdmin); // Flag should be set
      Assert.Single(context.ChatMessages); // Message persisted
    }

    [Fact]
    public async Task SendMessageAsync_Admin_SendsNotification_ToStudent()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var mockNotification = new Mock<INotificationService>();
      var service = new ChatService(context, mockNotification.Object);

      // Pre-existing thread for student
      var student = new User { Id = 5, FullName = "Student 5", Email = "s@test.com", PasswordHash = "hash" };
      context.Users.Add(student);
      context.ChatThreads.Add(new ChatThread { Id = 1, UserId = 5, User = student });
      await context.SaveChangesAsync();

      var dto = new SendMessageDto { MessageText = "Support reply", TargetUserId = 5 };

      // Act
      await service.SendMessageAsync(99, UserRole.Admin, dto);

      // Assert
      var thread = await context.ChatThreads.FindAsync(1);
      Assert.True(thread.HasUnreadForUser); // Flag for user set

      // Verify Notification sent to Student (Id 5)
      mockNotification.Verify(n => n.CreateAndSendAsync(
          5,
          "New Support Message",
          It.IsAny<string>(),
          NotificationType.NewChatMessage,
          1), Times.Once);
    }

    [Fact]
    public async Task GetThreadMessagesAsync_StudentViewing_ResetsUnreadForUser()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var mockNotification = new Mock<INotificationService>();
      var service = new ChatService(context, mockNotification.Object);

      var student = new User { Id = 10, FullName = "S", Email = "s@s.com", PasswordHash = "p" };
      context.Users.Add(student);

      var thread = new ChatThread
      {
        Id = 1,
        UserId = 10,
        User = student,
        HasUnreadForUser = true, // Admin previously replied
        HasUnreadForAdmin = true
      };

      context.ChatThreads.Add(thread);
      context.ChatMessages.Add(new ChatMessage { ThreadId = 1, SenderId = 10, MessageText = "Hi", Sender = student });
      await context.SaveChangesAsync();

      // Act
      // Student (Role: Student) requests their own history
      var messages = await service.GetThreadMessagesAsync(10, UserRole.Student);

      // Assert
      var updatedThread = await context.ChatThreads.FindAsync(1);
      Assert.False(updatedThread.HasUnreadForUser); // Should be reset
      Assert.True(updatedThread.HasUnreadForAdmin); // Should remain touched
      Assert.NotEmpty(messages);
    }

    [Fact]
    public async Task GetAdminThreadsAsync_ReturnsThreads_OrderedByUnread()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var mockNotification = new Mock<INotificationService>();
      var service = new ChatService(context, mockNotification.Object);

      var u1 = new User { Id = 1, FullName = "U1", Email = "1", PasswordHash = "p" };
      var u2 = new User { Id = 2, FullName = "U2", Email = "2", PasswordHash = "p" };
      context.Users.AddRange(u1, u2);

      // Thread 1: Read by admin (older)
      context.ChatThreads.Add(new ChatThread { Id = 1, UserId = 1, User = u1, HasUnreadForAdmin = false, LastMessageAt = DateTime.Now.AddHours(-1) });
      // Thread 2: Unread by admin (newer)
      context.ChatThreads.Add(new ChatThread { Id = 2, UserId = 2, User = u2, HasUnreadForAdmin = true, LastMessageAt = DateTime.Now });

      await context.SaveChangesAsync();

      // Act
      var threads = await service.GetAdminThreadsAsync();

      // Assert
      Assert.Equal(2, threads.Count());
      Assert.Equal(2, threads.First().ThreadId); // Thread 2 should be first because it is Unread
    }
  }
}
