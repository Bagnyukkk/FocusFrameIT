using FocusFrameAPI.Dtos.Forum;
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
  public class ForumController : ControllerBase
  {
    private readonly IForumService _forumService;

    public ForumController(IForumService forumService)
    {
      _forumService = forumService;
    }

    private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    private UserRole CurrentUserRole => Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value!);

    [HttpGet]
    public async Task<IActionResult> GetTopics([FromQuery] string filter = "all")
    {
      var topics = await _forumService.GetTopicsAsync(CurrentUserId, filter);
      return Ok(topics);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTopic([FromBody] CreateTopicDto dto)
    {
      var result = await _forumService.CreateTopicAsync(CurrentUserId, dto);
      return CreatedAtAction(nameof(GetTopics), new { id = result.Id }, result);
    }

    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] CreateCommentDto dto)
    {
      await _forumService.AddCommentAsync(CurrentUserId, id, dto);
      return Ok();
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikeTopic(int id)
    {
      await _forumService.ToggleLikeAsync(id);
      return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTopic(int id)
    {
      await _forumService.DeleteTopicAsync(CurrentUserId, CurrentUserRole, id);
      return NoContent();
    }
  }
}
