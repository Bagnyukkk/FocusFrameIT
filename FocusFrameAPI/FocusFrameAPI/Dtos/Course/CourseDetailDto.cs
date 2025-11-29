using FocusFrameAPI.Dtos.Lesson;

namespace FocusFrameAPI.Dtos.Course
{
  public class CourseDetailDto : CourseListDto
  {
    public int DurationMinutes { get; set; }
    public List<LessonListDto> Lessons { get; set; } = new();
  }
}
