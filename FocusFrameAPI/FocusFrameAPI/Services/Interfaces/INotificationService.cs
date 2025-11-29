using FocusFrameAPI.Dtos.Admin;
using FocusFrameAPI.Enums;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface INotificationService
  {
    Task CreateAndSendAsync(int userId, string title, string message, NotificationType type, int? referenceId = null);
    Task<IEnumerable<NotificationDto>> GetUnreadAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
    Task MarkAllAsReadAsync(int userId);
  }
}
