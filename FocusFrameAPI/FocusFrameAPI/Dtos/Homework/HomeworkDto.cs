using FocusFrameAPI.Enums;

namespace FocusFrameAPI.Dtos.Homework
{
  public class HomeworkDto
  {
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public HomeworkStatus Status { get; set; }
    public string AdminComment { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
  }
}
