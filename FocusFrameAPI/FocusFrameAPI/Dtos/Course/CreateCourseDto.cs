namespace FocusFrameAPI.Dtos.Course
{
  public class CreateCourseDto
  {
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string MentorName { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Beginner";
  }
}
