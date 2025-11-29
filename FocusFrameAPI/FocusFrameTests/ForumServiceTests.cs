using FocusFrameAPI.Dtos.Forum;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameAPI.Services.Interfaces;
using FocusFrameTests.Helpers;
using Moq;

namespace FocusFrameTests
{
  public class ForumServiceTests
  {
    [Fact]
    public async Task GetTopicsAsync_FilterMyPosts_ReturnsOnlyUserTopics()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var mockNotification = new Mock<INotificationService>();
      var service = new ForumService(context, mockNotification.Object);

      var user1 = new User { Id = 1, FullName = "User One", Email = "u1@test.com", PasswordHash = "hash" };
      var user2 = new User { Id = 2, FullName = "User Two", Email = "u2@test.com", PasswordHash = "hash" };

      context.Users.AddRange(user1, user2);
      context.ForumTopics.Add(new ForumTopic { Id = 1, Title = "Topic 1", AuthorId = 1, Author = user1 });
      context.ForumTopics.Add(new ForumTopic { Id = 2, Title = "Topic 2", AuthorId = 2, Author = user2 });
      await context.SaveChangesAsync();

      // Act
      var result = await service.GetTopicsAsync(1, "myposts");

      // Assert
      Assert.Single(result);
      Assert.Equal("Topic 1", result.First().Title);
    }

    [Fact]
    public async Task AddCommentAsync_IncrementsCount_And_SendsNotification_WhenNotAuthor()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var mockNotification = new Mock<INotificationService>();
      var service = new ForumService(context, mockNotification.Object);

      var author = new User { Id = 1, FullName = "Author", Email = "a@test.com", PasswordHash = "hash" };
      var commenter = new User { Id = 2, FullName = "Commenter", Email = "c@test.com", PasswordHash = "hash" };

      context.Users.AddRange(author, commenter);
      var topic = new ForumTopic { Id = 1, Title = "Help", AuthorId = 1, Author = author, CommentsCount = 0 };
      context.ForumTopics.Add(topic);
      await context.SaveChangesAsync();

      var commentDto = new CreateCommentDto { Content = "This is a reply." };

      // Act
      await service.AddCommentAsync(2, 1, commentDto); // User 2 comments on User 1's post

      // Assert
      var updatedTopic = await context.ForumTopics.FindAsync(1);
      Assert.Equal(1, updatedTopic.CommentsCount); // Check counter incremented

      // Verify Notification was sent to User 1
      mockNotification.Verify(n => n.CreateAndSendAsync(
          1,
          It.IsAny<string>(),
          It.Is<string>(s => s.Contains("Commenter")),
          NotificationType.System,
          1), Times.Once);
    }

    [Fact]
    public async Task DeleteTopicAsync_StudentCannotDeleteOthersTopic_ThrowsException()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var mockNotification = new Mock<INotificationService>();
      var service = new ForumService(context, mockNotification.Object);

      var author = new User { Id = 1, FullName = "Author", Email = "a@test.com", PasswordHash = "hash" };
      var otherStudent = new User { Id = 2, FullName = "Hacker", Email = "h@test.com", PasswordHash = "hash" };

      context.Users.AddRange(author, otherStudent);
      context.ForumTopics.Add(new ForumTopic { Id = 10, Title = "My Topic", AuthorId = 1, Author = author });
      await context.SaveChangesAsync();

      // Act & Assert
      await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
          service.DeleteTopicAsync(2, UserRole.Student, 10));
    }

    [Fact]
    public async Task DeleteTopicAsync_AdminCanDeleteAnyTopic()
    {
      // Arrange
      using var context = DbContextHelper.GetInMemoryDbContext();
      var mockNotification = new Mock<INotificationService>();
      var service = new ForumService(context, mockNotification.Object);

      context.Users.Add(new User { Id = 1, FullName = "Author", Email = "a@test.com", PasswordHash = "hash" });
      context.ForumTopics.Add(new ForumTopic { Id = 50, Title = "Bad Topic", AuthorId = 1 });
      await context.SaveChangesAsync();

      // Act
      await service.DeleteTopicAsync(99, UserRole.Admin, 50); // 99 is admin ID

      // Assert
      Assert.Empty(context.ForumTopics);
    }
  }
}
