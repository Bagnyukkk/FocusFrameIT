using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Course;
using FocusFrameAPI.Dtos.Lesson;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameAPI.Services
{
  public class CourseService : ICourseService
  {
    private readonly FocusFrameDbContext _context;

    public CourseService(FocusFrameDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<CourseListDto>> GetAllCoursesAsync()
    {
      // Retrieves basic info for the landing page/catalog
      return await _context.Courses
          .Select(c => new CourseListDto
          {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            MentorName = c.MentorName,
            PreviewImageUrl = c.PreviewImageUrl,
            Difficulty = c.Difficulty,
            Rating = c.Rating
          })
          .ToListAsync();
        }

    public async Task<CourseDetailDto?> GetCourseDetailsAsync(int courseId)
    {
      var course = await _context.Courses
          .Include(c => c.Lessons)
          .FirstOrDefaultAsync(c => c.Id == courseId);

      if (course == null) return null;

      return new CourseDetailDto
      {
        Id = course.Id,
        Title = course.Title,
        Description = course.Description,
        MentorName = course.MentorName,
        DurationMinutes = course.DurationMinutes,
        PreviewImageUrl = course.PreviewImageUrl,
        Difficulty = course.Difficulty,
        Rating = course.Rating,
        Lessons = course.Lessons.OrderBy(l => l.OrderIndex).Select(l => new LessonListDto
        {
          Id = l.Id,
          Title = l.Title,
          OrderIndex = l.OrderIndex,
          DurationMinutes = l.DurationMinutes
          // Sensitive content (VideoUrl) excluded
        }).ToList()
      };
    }

    public async Task<LessonContentDto?> GetLessonContentAsync(int userId, int lessonId)
    {
      var lesson = await _context.Lessons.FindAsync(lessonId);
      if (lesson == null) return null;

      // Security Check: Verify User is Enrolled in the Course
      var isEnrolled = await _context.Enrollments
          .AnyAsync(e => e.UserId == userId && e.CourseId == lesson.CourseId);

      // If not enrolled (and not Admin - simplified here), deny access to content
      if (!isEnrolled) return null;

      return new LessonContentDto
      {
        Id = lesson.Id,
        Title = lesson.Title,
        OrderIndex = lesson.OrderIndex,
        DurationMinutes = lesson.DurationMinutes,
        VideoUrl = lesson.VideoUrl, // Exposed only after check
        TextContent = lesson.TextContent,
        QuizJsonData = lesson.QuizJsonData
      };
    }

    public async Task<int> CreateCourseAsync(CreateCourseDto dto)
    {
      var course = new Course
      {
        Title = dto.Title,
        Description = dto.Description,
        MentorName = dto.MentorName,
        Difficulty = dto.Difficulty,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      _context.Courses.Add(course);
      await _context.SaveChangesAsync();
      return course.Id;
    }

    public async Task<int> CreateLessonAsync(CreateLessonDto dto)
    {
      var lesson = new Lesson
      {
        CourseId = dto.CourseId,
        Title = dto.Title,
        VideoUrl = dto.VideoUrl,
        TextContent = dto.TextContent,
        OrderIndex = dto.OrderIndex,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      _context.Lessons.Add(lesson);
      await _context.SaveChangesAsync();
      return lesson.Id;
    }
  }
}
