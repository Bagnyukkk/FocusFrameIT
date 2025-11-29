using FocusFrameAPI.Dtos.Admin;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface IAdminDashboardService
  {
    Task<AdminStatsDto> GetDashboardStatsAsync();
    Task<IEnumerable<StudentProgressSummaryDto>> GetStudentsWithProgressAsync();
  }
}
