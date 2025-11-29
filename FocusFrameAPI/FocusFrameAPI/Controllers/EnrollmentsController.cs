using FocusFrameAPI.Dtos.Lesson;
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
  public class EnrollmentsController : ControllerBase
  {
    private readonly ILearningService _learningService;

    public EnrollmentsController(ILearningService learningService)
    {
      _learningService = learningService;
    }

    // POST: api/enrollments/join/5
    [HttpPost("join/{courseId}")]
    public async Task<IActionResult> JoinCourse(int courseId)
    {
      var userId = GetCurrentUserId();
      await _learningService.JoinCourseAsync(userId, courseId);
      return Ok(new { message = "Enrolled successfully" });
    }

    // POST: api/enrollments/lessons/5/view
    [HttpPost("lessons/{lessonId}/view")]
    public async Task<IActionResult> MarkLessonViewed(int lessonId)
    {
      var userId = GetCurrentUserId();
      await _learningService.MarkLessonViewedAsync(userId, lessonId);
      return Ok();
    }

    // POST: api/enrollments/lessons/5/quiz
    [HttpPost("lessons/{lessonId}/quiz")]
    public async Task<IActionResult> SubmitQuiz(int lessonId, [FromBody] QuizSubmissionDto submission)
    {
      var userId = GetCurrentUserId();
      await _learningService.SubmitQuizAsync(userId, lessonId, submission);
      return Ok(new { message = "Quiz submitted" });
    }

    // Helper to get ID from JWT
    private int GetCurrentUserId()
    {
      var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      return int.Parse(idStr!);
    }
  }
}
