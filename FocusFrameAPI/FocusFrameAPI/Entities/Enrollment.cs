using FocusFrameAPI.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class Enrollment : BaseEntity
  {
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [ForeignKey("Course")]
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.InProgress;

    public bool IsCertificateIssued { get; set; }
    public DateTime? CompletionDate { get; set; }

    public ICollection<LessonProgress> LessonProgresses { get; set; } = [];
  }
}
