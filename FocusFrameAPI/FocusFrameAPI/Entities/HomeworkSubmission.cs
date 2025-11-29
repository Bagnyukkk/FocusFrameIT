using FocusFrameAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class HomeworkSubmission : BaseEntity
  {

    [ForeignKey("Lesson")]
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    [ForeignKey("User")]
    public int StudentId { get; set; }
    public User Student { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string AdminComment { get; set; } = string.Empty;

    public HomeworkStatus Status { get; set; } = HomeworkStatus.Pending;

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
  }
}
