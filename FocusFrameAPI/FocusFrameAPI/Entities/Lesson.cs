using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class Lesson : BaseEntity
  {
    [ForeignKey("Course")]
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public required string Title { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public string QuizJsonData { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int DurationMinutes { get; set; }

    public ICollection<HomeworkSubmission> HomeworkSubmissions { get; set; } = [];
  }
}
