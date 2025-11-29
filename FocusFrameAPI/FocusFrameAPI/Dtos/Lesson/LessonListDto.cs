namespace FocusFrameAPI.Dtos.Lesson
{
  public class LessonListDto
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int DurationMinutes { get; set; }
  }
}
