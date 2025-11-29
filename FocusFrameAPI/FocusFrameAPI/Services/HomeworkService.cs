using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Homework;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameAPI.Services
{
  public class HomeworkService : IHomeworkService
  {
    private readonly FocusFrameDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IWebHostEnvironment _environment;

    public HomeworkService(
        FocusFrameDbContext context,
        INotificationService notificationService,
        IWebHostEnvironment environment)
    {
      _context = context;
      _notificationService = notificationService; // 
      _environment = environment;
    }

    public async Task<HomeworkDto> SubmitHomeworkAsync(int userId, int lessonId, IFormFile file)
    {
      // 1. File Handling (Local Storage Simulation)
      var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "homeworks");
      if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

      var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
      var filePath = Path.Combine(uploadsFolder, uniqueFileName);
      var fileUrl = $"/uploads/homeworks/{uniqueFileName}";

      using (var fileStream = new FileStream(filePath, FileMode.Create))
      {
        await file.CopyToAsync(fileStream);
      }

      // 2. Create Entity
      var submission = new HomeworkSubmission
      {
        StudentId = userId,
        LessonId = lessonId,
        FileName = file.FileName,
        FileUrl = fileUrl,
        FileSize = file.Length,
        Status = HomeworkStatus.Pending,
        SubmittedAt = DateTime.UtcNow
      };

      _context.HomeworkSubmissions.Add(submission);
      await _context.SaveChangesAsync();

      // 3. Notification Logic: Notify Admins
      // Fetch lesson details for the message
      var lesson = await _context.Lessons.FindAsync(lessonId);
      var student = await _context.Users.FindAsync(userId);

      // Find all admins
      var admins = await _context.Users
          .Where(u => u.Role == UserRole.Admin)
          .ToListAsync();

      foreach (var admin in admins)
      {
        await _notificationService.CreateAndSendAsync(
            userId: admin.Id,
            title: "New Homework Submitted",
            message: $"Student {student?.FullName ?? "Unknown"} submitted homework for lesson: {lesson?.Title}",
            type: NotificationType.System,
            referenceId: submission.Id
        );
      }

      return MapToDto(submission, lesson?.Title, student?.FullName);
    }

    public async Task<IEnumerable<HomeworkDto>> GetPendingHomeworksAsync()
    {
      var pending = await _context.HomeworkSubmissions
          .Include(h => h.Student)
          .Include(h => h.Lesson)
          .Where(h => h.Status == HomeworkStatus.Pending)
          .OrderByDescending(h => h.SubmittedAt)
          .ToListAsync();

      return pending.Select(h => MapToDto(h, h.Lesson.Title, h.Student.FullName));
    }

    public async Task<HomeworkDto> GradeHomeworkAsync(int homeworkId, GradeHomeworkDto gradeDto)
    {
      var submission = await _context.HomeworkSubmissions
          .Include(h => h.Lesson)
          .FirstOrDefaultAsync(h => h.Id == homeworkId);

      if (submission == null) throw new KeyNotFoundException("Homework not found");

      // 1. Update Status and Comment
      submission.Status = gradeDto.Status;
      submission.AdminComment = gradeDto.AdminComment;
      submission.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      // 2. Notification Logic: Notify Student
      await _notificationService.CreateAndSendAsync(
          userId: submission.StudentId,
          title: "Homework Graded",
          message: $"Your homework for {submission.Lesson.Title} has been {gradeDto.Status}. Comment: {gradeDto.AdminComment}",
          type: NotificationType.System,
          referenceId: submission.Id
      );

      return MapToDto(submission, submission.Lesson.Title, "Student");
    }

    private static HomeworkDto MapToDto(HomeworkSubmission entity, string? lessonTitle, string? studentName)
    {
      return new HomeworkDto
      {
        Id = entity.Id,
        LessonId = entity.LessonId,
        LessonTitle = lessonTitle ?? "Unknown Lesson",
        StudentId = entity.StudentId,
        StudentName = studentName ?? "Unknown Student",
        FileName = entity.FileName,
        FileUrl = entity.FileUrl,
        Status = entity.Status,
        AdminComment = entity.AdminComment,
        SubmittedAt = entity.SubmittedAt
      };
    }
  }
}
