using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class LessonProgress : BaseEntity
  {

    [ForeignKey("Enrollment")]
    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;

    [ForeignKey("Lesson")]
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? QuizScorePercent { get; set; }
    public bool QuizPassed { get; set; }
  }
}
