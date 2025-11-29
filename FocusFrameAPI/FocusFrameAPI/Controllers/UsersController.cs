using FocusFrameAPI.Dtos.User;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FocusFrameAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UsersController : ControllerBase
  {
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
      _userService = userService;
    }

    // GET: api/users/me
    // Accessible by: Student, Admin
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMyProfile()
    {
      try
      {
        var userId = GetUserIdFromClaims();
        var profile = await _userService.GetUserProfileAsync(userId);
        return Ok(profile);
      }
      catch (KeyNotFoundException)
      {
        return NotFound("User profile not found.");
      }
    }

    // PUT: api/users/me
    // Accessible by: Student, Admin
    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateMyProfile([FromBody] UpdateProfileDto dto)
    {
      try
      {
        var userId = GetUserIdFromClaims();
        var updatedProfile = await _userService.UpdateUserProfileAsync(userId, dto);
        return Ok(updatedProfile);
      }
      catch (KeyNotFoundException)
      {
        return NotFound("User profile not found.");
      }
    }

    // GET: api/users/students
    // Accessible by: Admin Only
    [Authorize(Roles = "Admin")]
    [HttpGet("students")]
    public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetAllStudents()
    {
      var students = await _userService.GetAllStudentsAsync();
      return Ok(students);
    }

    // DELETE: api/users/{id}
    // Accessible by: Admin Only
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
      try
      {
        await _userService.DeleteUserAsync(id);
        return NoContent();
      }
      catch (KeyNotFoundException)
      {
        return NotFound("User to delete not found.");
      }
    }

    // Helper method to extract the User ID from the JWT token claims
    private int GetUserIdFromClaims()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (int.TryParse(userIdClaim, out int userId))
      {
        return userId;
      }
      throw new UnauthorizedAccessException("Invalid token claims");
    }
  }
}
