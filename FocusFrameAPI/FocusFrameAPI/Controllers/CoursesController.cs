using FocusFrameAPI.Dtos.Course;
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
  public class CoursesController : ControllerBase
  {
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
      _courseService = courseService;
    }

    // GET: api/courses
    [HttpGet]
    [AllowAnonymous] // Landing page might need this public
    public async Task<IActionResult> GetAll()
    {
      var courses = await _courseService.GetAllCoursesAsync();
      return Ok(courses);
    }

    // GET: api/courses/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetails(int id)
    {
      var course = await _courseService.GetCourseDetailsAsync(id);
      if (course == null) return NotFound();
      return Ok(course);
    }

    // GET: api/courses/lessons/5/content
    // SECURE: Returns video URL only if enrolled
    [HttpGet("lessons/{lessonId}/content")]
    public async Task<IActionResult> GetLessonContent(int lessonId)
    {
      var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

      var content = await _courseService.GetLessonContentAsync(userId, lessonId);

      if (content == null)
        return StatusCode(403, "You must be enrolled in the course to view this content.");

      return Ok(content);
    }

    // POST: api/courses (Admin Only)
    [HttpPost]
    [Authorize(Roles = "Admin")] // Checks UserRole.Admin
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
      var id = await _courseService.CreateCourseAsync(dto);
      return CreatedAtAction(nameof(GetDetails), new { id }, new { id });
    }
  }
}
