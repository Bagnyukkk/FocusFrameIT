using FocusFrameAPI.Entities;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface INotificationClient
  {
    Task ReceiveNotification(Notification notification);
  }
}
