using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Chat;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameAPI.Services
{
  public class ChatService : IChatService
  {
    private readonly FocusFrameDbContext _context;
    private readonly INotificationService _notificationService;

    public ChatService(FocusFrameDbContext context, INotificationService notificationService)
    {
      _context = context;
      _notificationService = notificationService;
    }

    public async Task SendMessageAsync(int senderId, UserRole senderRole, SendMessageDto dto)
    {
      ChatThread thread;
      int recipientId;

      if (senderRole == UserRole.Student)
      {
        // Student sending: Find/Create their own thread 
        thread = await _context.ChatThreads
            .FirstOrDefaultAsync(t => t.UserId == senderId);

        if (thread == null)
        {
          thread = new ChatThread { UserId = senderId, LastMessageAt = DateTime.UtcNow };
          _context.ChatThreads.Add(thread);
          await _context.SaveChangesAsync();
        }

        // Student messages flag "Unread for Admin" [cite: 192]
        thread.HasUnreadForAdmin = true;

        // Note: We don't strictly know which Admin ID to notify, usually this goes to a group
        // For simplicity, we assume we notify all admins or a system channel (implementation specific)
        // Here we will skip direct ID notification for Admin group unless an Admin ID is known
      }
      else
      {
        // Admin sending: Must target a specific student [cite: 123]
        if (!dto.TargetUserId.HasValue)
          throw new ArgumentException("Admin must specify TargetUserId");

        recipientId = dto.TargetUserId.Value;
        thread = await _context.ChatThreads
            .FirstOrDefaultAsync(t => t.UserId == recipientId);

        if (thread == null) throw new KeyNotFoundException("Thread not found for this student");

        // Admin messages flag "Unread for User" [cite: 193]
        thread.HasUnreadForUser = true;

        // Notify the Student [cite: 124]
        await _notificationService.CreateAndSendAsync(
            recipientId,
            "New Support Message",
            $"Support: {dto.MessageText.Substring(0, Math.Min(20, dto.MessageText.Length))}...",
            NotificationType.NewChatMessage, // Using Message type
            thread.Id
        );
      }

      // Create the message 
      var message = new ChatMessage
      {
        ThreadId = thread.Id,
        SenderId = senderId,
        MessageText = dto.MessageText,
        SentAt = DateTime.UtcNow,
        IsRead = false
      };

      thread.LastMessageAt = DateTime.UtcNow;
      _context.ChatMessages.Add(message);
      await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ChatMessageDto>> GetThreadMessagesAsync(int requestorId, UserRole requestorRole, int? targetStudentId = null)
    {
      int studentIdToCheck = requestorRole == UserRole.Student ? requestorId : (targetStudentId ?? 0);

      var thread = await _context.ChatThreads
          .Include(t => t.Messages)
          .ThenInclude(m => m.Sender)
          .FirstOrDefaultAsync(t => t.UserId == studentIdToCheck);

      if (thread == null) return new List<ChatMessageDto>();

      // Mark as read based on who is reading [cite: 189]
      if (requestorRole == UserRole.Student)
      {
        thread.HasUnreadForUser = false;
      }
      else
      {
        thread.HasUnreadForAdmin = false;
      }
      await _context.SaveChangesAsync();

      return thread.Messages.Select(m => new ChatMessageDto
      {
        Id = m.Id,
        SenderId = m.SenderId,
        SenderName = m.Sender.FullName,
        MessageText = m.MessageText,
        SentAt = m.SentAt,
        IsRead = m.IsRead
      }).OrderBy(m => m.SentAt);
    }

    public async Task<IEnumerable<ChatThreadDto>> GetAdminThreadsAsync()
    {
      // Admin view: List all threads, prioritize unread [cite: 121]
      return await _context.ChatThreads
          .Include(t => t.User)
          .OrderByDescending(t => t.HasUnreadForAdmin) // Unread first
          .ThenByDescending(t => t.LastMessageAt)
          .Select(t => new ChatThreadDto
          {
            ThreadId = t.Id,
            StudentId = t.UserId,
            StudentName = t.User.FullName,
            HasUnread = t.HasUnreadForAdmin, // [cite: 192]
            LastMessageAt = t.LastMessageAt
          })
          .ToListAsync();
    }
  }
}
