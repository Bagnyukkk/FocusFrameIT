using FocusFrameAPI.Dtos.Course;
using FocusFrameAPI.Dtos.Lesson;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface ICourseService
  {
    Task<IEnumerable<CourseListDto>> GetAllCoursesAsync();
    Task<CourseDetailDto?> GetCourseDetailsAsync(int courseId);
    Task<LessonContentDto?> GetLessonContentAsync(int userId, int lessonId);
    Task<int> CreateCourseAsync(CreateCourseDto dto);
    Task<int> CreateLessonAsync(CreateLessonDto dto);
  }
}
