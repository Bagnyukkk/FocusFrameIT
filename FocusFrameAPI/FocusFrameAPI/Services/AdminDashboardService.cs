using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Admin;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameAPI.Services
{
  public class AdminDashboardService : IAdminDashboardService
  {
    private readonly FocusFrameDbContext _context;

    public AdminDashboardService(FocusFrameDbContext context)
    {
      _context = context;
    }

    public async Task<AdminStatsDto> GetDashboardStatsAsync()
    {
      // Aggregating data for dashboard [cite: 203]
      var totalUsers = await _context.Users.CountAsync();
      var totalCourses = await _context.Courses.CountAsync();

      // Count pending homework based on enum Status [cite: 62]
      var pendingHomework = await _context.HomeworkSubmissions
          .CountAsync(h => h.Status == HomeworkStatus.Pending);

      return new AdminStatsDto
      {
        TotalUsers = totalUsers,
        TotalCourses = totalCourses,
        PendingHomeworkCount = pendingHomework
      };
    }

    public async Task<IEnumerable<StudentProgressSummaryDto>> GetStudentsWithProgressAsync()
    {
      // Retrieve students and their enrollments to calculate progress
      var students = await _context.Users
          .Where(u => u.Role == UserRole.Student) // Filter by Role [cite: 81]
          .Include(u => u.Enrollments)
              .ThenInclude(e => e.LessonProgresses)
          .AsNoTracking()
          .ToListAsync();

      var result = students.Select(student =>
      {
        // Calculate logic based on enrollments [cite: 49, 71]
        var completedCourses = student.Enrollments.Count(e => e.Status == EnrollmentStatus.Completed);

        // Calculate overall completion percentage (Total Completed Lessons / Total Lessons in Enrolled Courses)
        // Note: For exact precision, we would need to join with Course.Lessons.Count. 
        // This is a simplified progress calculation based on tracked progress entries.
        var totalProgressEntries = student.Enrollments.Sum(e => e.LessonProgresses.Count);
        var completedLessons = student.Enrollments
            .SelectMany(e => e.LessonProgresses)
            .Count(lp => lp.IsCompleted);

        double progressPercent = totalProgressEntries > 0
            ? (double)completedLessons / totalProgressEntries * 100
            : 0;

        return new StudentProgressSummaryDto
        {
          UserId = student.Id,
          FullName = student.FullName,
          Email = student.Email,
          CoursesEnrolled = student.Enrollments.Count,
          CoursesCompleted = completedCourses,
          OverallProgressPercent = Math.Round(progressPercent, 1)
        };
      });

      return result;
    }
  }
}
