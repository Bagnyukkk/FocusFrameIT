using FocusFrameAPI.Dtos.Homework;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FocusFrameAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class HomeworksController : ControllerBase
  {
    private readonly IHomeworkService _homeworkService;

    public HomeworksController(IHomeworkService homeworkService)
    {
      _homeworkService = homeworkService;
    }

    // POST: api/homeworks/submit/{lessonId}
    [HttpPost("submit/{lessonId}")]
    public async Task<ActionResult<HomeworkDto>> SubmitHomework(int lessonId, IFormFile file)
    {
      if (file == null || file.Length == 0)
        return BadRequest("File is required.");

      // Extract Student ID from the current User Claims
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
      if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        return Unauthorized();

      try
      {
        var result = await _homeworkService.SubmitHomeworkAsync(userId, lessonId, file);
        return CreatedAtAction(nameof(GetPendingHomeworks), new { id = result.Id }, result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Internal server error: {ex.Message}");
      }
    }

    // GET: api/homeworks/pending
    // Only Admins can see the queue of pending homework
    [HttpGet("pending")]
    [Authorize(Roles = "Admin")] // Requires Role property on User [cite: 81]
    public async Task<ActionResult<IEnumerable<HomeworkDto>>> GetPendingHomeworks()
    {
      var result = await _homeworkService.GetPendingHomeworksAsync();
      return Ok(result);
    }

    // PUT: api/homeworks/{id}/grade
    // Only Admins can grade homework
    [HttpPut("{id}/grade")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HomeworkDto>> GradeHomework(int id, [FromBody] GradeHomeworkDto gradeDto)
    {
      if (gradeDto.Status == HomeworkStatus.Pending)
        return BadRequest("You must change status to Approved or Rejected.");

      try
      {
        var result = await _homeworkService.GradeHomeworkAsync(id, gradeDto);
        return Ok(result);
      }
      catch (KeyNotFoundException)
      {
        return NotFound();
      }
    }
  }
}
