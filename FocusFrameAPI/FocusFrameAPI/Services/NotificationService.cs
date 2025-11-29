using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Admin;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameAPI.Services
{
  public class NotificationService : INotificationService
  {
    private readonly FocusFrameDbContext _context;
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public NotificationService(FocusFrameDbContext context, IHubContext<NotificationHub, INotificationClient> hubContext)
    {
      _context = context;
      _hubContext = hubContext;
    }

    public async Task CreateAndSendAsync(int userId, string title, string message, NotificationType type, int? referenceId = null)
    {
      var notification = new Notification
      {
        UserId = userId,
        Title = title,
        Message = message,
        Type = type,
        ReferenceId = referenceId,
        IsRead = false,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      _context.Notifications.Add(notification);
      await _context.SaveChangesAsync();

      await _hubContext.Clients.User(userId.ToString())
                       .ReceiveNotification(notification);
    }

    public async Task<IEnumerable<NotificationDto>> GetUnreadAsync(int userId)
    {
      return await _context.Notifications
          .Where(n => n.UserId == userId && !n.IsRead)
          .OrderByDescending(n => n.CreatedAt)
          .Select(n => new NotificationDto
          {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type.ToString(),
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            ReferenceId = n.ReferenceId
          })
          .ToListAsync();
    }

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
      var notification = await _context.Notifications
          .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

      if (notification != null)
      {
        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
      }
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
      var notifications = await _context.Notifications
          .Where(n => n.UserId == userId && !n.IsRead)
          .ToListAsync();

      if (notifications.Any())
      {
        foreach (var note in notifications)
        {
          note.IsRead = true;
          note.ReadAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
      }
    }
  }
}
