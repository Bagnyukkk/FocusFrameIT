using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Homework;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FocusFrameTests
{
  public class HomeworkServiceTests
  {
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<IWebHostEnvironment> _mockEnvironment;
    private readonly DbContextOptions<FocusFrameDbContext> _dbOptions;

    public HomeworkServiceTests()
    {
      _mockNotificationService = new Mock<INotificationService>();
      _mockEnvironment = new Mock<IWebHostEnvironment>();

      // Setup In-Memory Database with a unique name for each test run to ensure isolation
      _dbOptions = new DbContextOptionsBuilder<FocusFrameDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      // Setup a temporary path for file upload simulation
      var tempPath = Path.GetTempPath();
      _mockEnvironment.Setup(e => e.WebRootPath).Returns(tempPath);
    }

    // Helper to create a clean context
    private FocusFrameDbContext CreateContext() => new FocusFrameDbContext(_dbOptions);

    [Fact]
    public async Task SubmitHomeworkAsync_ValidInput_SavesToDbAndNotifiesAdmin()
    {
      // Arrange
      using var context = CreateContext();
      var service = new HomeworkService(context, _mockNotificationService.Object, _mockEnvironment.Object);

      // Seed Data
      var student = new User { Id = 1, FullName = "Student One", Role = UserRole.Student, Email = "student@test.com", PasswordHash = "hash" };
      var admin = new User { Id = 2, FullName = "Admin User", Role = UserRole.Admin, Email = "admin@test.com", PasswordHash = "hash" };
      var lesson = new Lesson { Id = 10, Title = "Intro to Photography", QuizJsonData = "{}", OrderIndex = 1 };

      context.Users.AddRange(student, admin);
      context.Lessons.Add(lesson);
      await context.SaveChangesAsync();

      // Mock File
      var fileMock = new Mock<IFormFile>();
      var content = "Fake PDF Content";
      var fileName = "homework.pdf";
      var ms = new MemoryStream();
      var writer = new StreamWriter(ms);
      writer.Write(content);
      writer.Flush();
      ms.Position = 0;

      fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
      fileMock.Setup(f => f.FileName).Returns(fileName);
      fileMock.Setup(f => f.Length).Returns(ms.Length);
      fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
          .Returns(Task.CompletedTask);

      // Act
      var result = await service.SubmitHomeworkAsync(student.Id, lesson.Id, fileMock.Object);

      // Assert
      // 1. Check Return DTO
      Assert.Equal(fileName, result.FileName);
      Assert.Equal(HomeworkStatus.Pending, result.Status);

      // 2. Check Database
      var dbSubmission = await context.HomeworkSubmissions.FirstOrDefaultAsync();
      Assert.NotNull(dbSubmission);
      Assert.Equal(student.Id, dbSubmission.StudentId);
      Assert.Contains("homeworks", dbSubmission.FileUrl); // Basic check for path logic

      // 3. Verify Notification sent to Admin
      _mockNotificationService.Verify(n => n.CreateAndSendAsync(
          admin.Id,
          "New Homework Submitted",
          It.Is<string>(s => s.Contains("Student One") && s.Contains("Intro to Photography")),
          NotificationType.System,
          dbSubmission.Id),
          Times.Once);
    }

    [Fact]
    public async Task GetPendingHomeworksAsync_ReturnsOnlyPendingSubmissions()
    {
      // Arrange
      using var context = CreateContext();
      var service = new HomeworkService(context, _mockNotificationService.Object, _mockEnvironment.Object);

      var student = new User { Id = 1, FullName = "Student", Role = UserRole.Student, Email = "s@t.com", PasswordHash = "h" };
      var lesson = new Lesson { Id = 1, Title = "L1", QuizJsonData = "{}", OrderIndex = 1 };

      context.Users.Add(student);
      context.Lessons.Add(lesson);

      var pendingHw = new HomeworkSubmission { Id = 1, StudentId = 1, LessonId = 1, Status = HomeworkStatus.Pending, SubmittedAt = DateTime.UtcNow };
      var approvedHw = new HomeworkSubmission { Id = 2, StudentId = 1, LessonId = 1, Status = HomeworkStatus.Approved, SubmittedAt = DateTime.UtcNow };

      context.HomeworkSubmissions.AddRange(pendingHw, approvedHw);
      await context.SaveChangesAsync();

      // Act
      var result = await service.GetPendingHomeworksAsync();

      // Assert
      Assert.Single(result); // Should only get 1
      Assert.Equal(pendingHw.Id, result.First().Id);
    }

    [Fact]
    public async Task GradeHomeworkAsync_ValidId_UpdatesStatusAndNotifiesStudent()
    {
      // Arrange
      using var context = CreateContext();
      var service = new HomeworkService(context, _mockNotificationService.Object, _mockEnvironment.Object);

      var student = new User { Id = 99, FullName = "Student 99", Role = UserRole.Student, Email = "s@t.com", PasswordHash = "h" };
      var lesson = new Lesson { Id = 5, Title = "Lighting", QuizJsonData = "{}", OrderIndex = 1 };
      var submission = new HomeworkSubmission { Id = 10, StudentId = 99, LessonId = 5, Status = HomeworkStatus.Pending };

      context.Users.Add(student);
      context.Lessons.Add(lesson);
      context.HomeworkSubmissions.Add(submission);
      await context.SaveChangesAsync();

      var gradeDto = new GradeHomeworkDto
      {
        Status = HomeworkStatus.Rejected,
        AdminComment = "Please fix lighting"
      };

      // Act
      var result = await service.GradeHomeworkAsync(10, gradeDto);

      // Assert
      // 1. Verify DB Update
      var updatedSubmission = await context.HomeworkSubmissions.FindAsync(10);
      Assert.Equal(HomeworkStatus.Rejected, updatedSubmission.Status);
      Assert.Equal("Please fix lighting", updatedSubmission.AdminComment);

      // 2. Verify Notification to Student
      _mockNotificationService.Verify(n => n.CreateAndSendAsync(
          student.Id,
          "Homework Graded",
          It.Is<string>(s => s.Contains("Rejected") && s.Contains("Please fix lighting")),
          NotificationType.System,
          submission.Id),
          Times.Once);
    }

    [Fact]
    public async Task GradeHomeworkAsync_InvalidId_ThrowsKeyNotFoundException()
    {
      // Arrange
      using var context = CreateContext();
      var service = new HomeworkService(context, _mockNotificationService.Object, _mockEnvironment.Object);

      var gradeDto = new GradeHomeworkDto { Status = HomeworkStatus.Approved };

      // Act & Assert
      await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GradeHomeworkAsync(999, gradeDto));
    }
  }
}