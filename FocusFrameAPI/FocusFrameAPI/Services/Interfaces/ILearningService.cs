using FocusFrameAPI.Dtos.Lesson;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface ILearningService
  {
    Task JoinCourseAsync(int userId, int courseId);
    Task MarkLessonViewedAsync(int userId, int lessonId);
    Task SubmitQuizAsync(int userId, int lessonId, QuizSubmissionDto submission);
  }
}
