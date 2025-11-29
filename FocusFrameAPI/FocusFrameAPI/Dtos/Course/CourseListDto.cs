namespace FocusFrameAPI.Dtos.Course
{
  public class CourseListDto
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PreviewImageUrl { get; set; } = string.Empty;
    public string MentorName { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public double Rating { get; set; }
  }
}
