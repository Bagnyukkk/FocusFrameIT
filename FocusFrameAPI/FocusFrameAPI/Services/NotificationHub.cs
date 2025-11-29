using FocusFrameAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FocusFrameAPI.Services
{
  [Authorize]
  public class NotificationHub : Hub<INotificationClient>
  {
  }
}
