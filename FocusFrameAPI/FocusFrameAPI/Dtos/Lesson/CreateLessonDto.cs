namespace FocusFrameAPI.Dtos.Lesson
{
  public class CreateLessonDto
  {
    public int CourseId { get; set; }
    public required string Title { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
  }
}
