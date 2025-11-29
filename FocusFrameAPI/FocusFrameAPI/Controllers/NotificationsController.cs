using FocusFrameAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FocusFrameAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class NotificationsController : ControllerBase
  {
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
      _notificationService = notificationService;
    }

    private int GetCurrentUserId()
    {
      var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
      if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
      {
        return userId;
      }
      throw new UnauthorizedAccessException("Invalid user token");
    }

    [HttpGet]
    public async Task<IActionResult> GetUnreadNotifications()
    {
      var userId = GetCurrentUserId();
      var notifications = await _notificationService.GetUnreadAsync(userId);
      return Ok(notifications);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
      var userId = GetCurrentUserId();
      await _notificationService.MarkAsReadAsync(id, userId);
      return NoContent();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
      var userId = GetCurrentUserId();
      await _notificationService.MarkAllAsReadAsync(userId);
      return NoContent();
    }
  }
}
