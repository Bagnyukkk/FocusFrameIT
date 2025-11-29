namespace FocusFrameAPI.Dtos.User
{
  public class UpdateProfileDto
  {
    public string FullName { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
  }
}
