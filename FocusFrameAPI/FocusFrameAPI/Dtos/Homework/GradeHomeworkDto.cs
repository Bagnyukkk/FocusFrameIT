using FocusFrameAPI.Enums;

namespace FocusFrameAPI.Dtos.Homework
{
  public class GradeHomeworkDto
  {
    public HomeworkStatus Status { get; set; }
    public string AdminComment { get; set; } = string.Empty;
  }
}
