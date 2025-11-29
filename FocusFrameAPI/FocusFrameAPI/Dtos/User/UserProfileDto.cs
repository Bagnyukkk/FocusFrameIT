namespace FocusFrameAPI.Dtos.User
{
  public class UserProfileDto
  {
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
  }
}
