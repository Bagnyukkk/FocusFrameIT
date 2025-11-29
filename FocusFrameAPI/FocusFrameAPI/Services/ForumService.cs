using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Forum;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameAPI.Services
{
  public class ForumService : IForumService
  {
    private readonly FocusFrameDbContext _context;
    private readonly INotificationService _notificationService;

    public ForumService(FocusFrameDbContext context, INotificationService notificationService)
    {
      _context = context;
      _notificationService = notificationService;
    }

    public async Task<IEnumerable<TopicDto>> GetTopicsAsync(int userId, string filter)
    {
      var query = _context.ForumTopics.Include(t => t.Author).AsQueryable();

      // Implements filtering logic described in Forum specifications [cite: 79]
      if (filter?.ToLower() == "myposts")
      {
        query = query.Where(t => t.AuthorId == userId);
      }
      else if (filter?.ToLower() == "popular")
      {
        query = query.OrderByDescending(t => t.LikesCount);
      }
      else
      {
        query = query.OrderByDescending(t => t.CreatedAt);
      }

      return await query.Select(t => new TopicDto
      {
        Id = t.Id,
        Title = t.Title,
        AuthorName = t.Author.FullName, // [cite: 210]
        LikesCount = t.LikesCount,      // [cite: 211]
        CommentsCount = t.CommentsCount, // [cite: 212]
        CreatedAt = t.CreatedAt
      }).ToListAsync();
    }

    public async Task<TopicDto> CreateTopicAsync(int userId, CreateTopicDto dto)
    {
      var topic = new ForumTopic
      {
        AuthorId = userId,
        Title = dto.Title,
        Content = dto.Content,
        LikesCount = 0,
        CommentsCount = 0
      };

      _context.ForumTopics.Add(topic);
      await _context.SaveChangesAsync();

      // Return DTO for immediate UI update
      var author = await _context.Users.FindAsync(userId);
      return new TopicDto
      {
        Id = topic.Id,
        Title = topic.Title,
        AuthorName = author?.FullName ?? "Unknown",
        CreatedAt = topic.CreatedAt
      };
    }

    public async Task AddCommentAsync(int userId, int topicId, CreateCommentDto dto)
    {
      var topic = await _context.ForumTopics.Include(t => t.Author)
                              .FirstOrDefaultAsync(t => t.Id == topicId);
      if (topic == null) throw new KeyNotFoundException("Topic not found");

      var user = await _context.Users.FindAsync(userId);

      // 1. Create Comment 
      var comment = new ForumComment
      {
        TopicId = topicId,
        AuthorId = userId,
        Content = dto.Content
      };

      // 2. Increment Counter
      topic.CommentsCount++; // [cite: 212]

      _context.ForumComments.Add(comment);

      // 3. Notification Logic: Notify author if commenter is not the author
      if (topic.AuthorId != userId)
      {
        // Using NotificationService from provided source [cite: 155]
        await _notificationService.CreateAndSendAsync(
            topic.AuthorId,
            "New Comment",
            $"{user?.FullName ?? "Someone"} commented on your post '{topic.Title}'",
            NotificationType.System, // Assuming System or Forum type
            topic.Id
        );
      }

      await _context.SaveChangesAsync();
    }

    public async Task ToggleLikeAsync(int topicId)
    {
      var topic = await _context.ForumTopics.FindAsync(topicId);
      if (topic == null) throw new KeyNotFoundException("Topic not found");

      topic.LikesCount++;
      await _context.SaveChangesAsync();
    }

    public async Task DeleteTopicAsync(int userId, UserRole role, int topicId)
    {
      var topic = await _context.ForumTopics.FindAsync(topicId);
      if (topic == null) return;

      if (role == UserRole.Admin || topic.AuthorId == userId)
      {
        _context.ForumTopics.Remove(topic);
        await _context.SaveChangesAsync();
      }
      else
      {
        throw new UnauthorizedAccessException("Cannot delete this topic");
      }
    }
  }
}
