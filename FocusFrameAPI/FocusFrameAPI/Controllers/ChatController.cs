using FocusFrameAPI.Dtos.Chat;
using FocusFrameAPI.Enums;
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
  public class ChatController : ControllerBase
  {
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
      _chatService = chatService;
    }

    private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    private UserRole CurrentUserRole => Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value!);

    // Get history (Student sees own, Admin must pass ?studentId=X)
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int? studentId)
    {
      if (CurrentUserRole == UserRole.Admin && studentId == null)
        return BadRequest("Admin must provide studentId");

      var messages = await _chatService.GetThreadMessagesAsync(CurrentUserId, CurrentUserRole, studentId);
      return Ok(messages);
    }

    // Send a message
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
    {
      await _chatService.SendMessageAsync(CurrentUserId, CurrentUserRole, dto);
      return Ok();
    }

    // Admin Only: Get list of active threads
    [HttpGet("admin/threads")]
    [Authorize(Roles = "Admin")] // [cite: 23]
    public async Task<IActionResult> GetThreads()
    {
      var threads = await _chatService.GetAdminThreadsAsync();
      return Ok(threads);
    }
  }
}
