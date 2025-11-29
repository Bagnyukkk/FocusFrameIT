using System.ComponentModel.DataAnnotations;

namespace FocusFrameAPI.Dtos.Auth
{
  public class RegisterDto
  {
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;
  }
}
