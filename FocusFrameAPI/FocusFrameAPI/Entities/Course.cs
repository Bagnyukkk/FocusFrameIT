using System.ComponentModel.DataAnnotations;

namespace FocusFrameAPI.Entities
{
  public class Course : BaseEntity
  {
    public required string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string MentorName { get; set; } = string.Empty;
    public string PreviewImageUrl { get; set; } = string.Empty;
    public string MentorPhotoUrl { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public double Rating { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
  }
}
