using FocusFrameAPI.Dtos.Homework;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface IHomeworkService
  {
    Task<HomeworkDto> SubmitHomeworkAsync(int userId, int lessonId, IFormFile file);
    Task<IEnumerable<HomeworkDto>> GetPendingHomeworksAsync();
    Task<HomeworkDto> GradeHomeworkAsync(int homeworkId, GradeHomeworkDto gradeDto);
  }
}
