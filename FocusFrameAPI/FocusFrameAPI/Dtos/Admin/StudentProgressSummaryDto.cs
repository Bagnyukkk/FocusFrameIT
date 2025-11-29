namespace FocusFrameAPI.Dtos.Admin
{
  public class StudentProgressSummaryDto
  {
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CoursesEnrolled { get; set; }
    public int CoursesCompleted { get; set; }
    public double OverallProgressPercent { get; set; }
  }
}
