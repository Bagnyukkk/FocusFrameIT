namespace FocusFrameAPI.Dtos.Lesson
{
  public class LessonContentDto : LessonListDto
  {
    public string VideoUrl { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public string QuizJsonData { get; set; } = string.Empty;
  }
}
