using FocusFrameAPI.Dtos.Admin;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FocusFrameAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize(Roles = nameof(UserRole.Admin))]
  public class AdminController : ControllerBase
  {
    private readonly IAdminDashboardService _adminService;

    public AdminController(IAdminDashboardService adminService)
    {
      _adminService = adminService;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<AdminStatsDto>> GetDashboardStats()
    {
      var stats = await _adminService.GetDashboardStatsAsync();
      return Ok(stats);
    }

    [HttpGet("students")]
    public async Task<ActionResult<IEnumerable<StudentProgressSummaryDto>>> GetStudentsList()
    {
      var students = await _adminService.GetStudentsWithProgressAsync();
      return Ok(students);
    }
  }
}
