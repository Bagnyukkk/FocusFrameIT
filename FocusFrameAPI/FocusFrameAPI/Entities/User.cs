using FocusFrameAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace FocusFrameAPI.Entities
{
  public class User : BaseEntity
  {

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public required string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Student;

    public string FullName { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<HomeworkSubmission> HomeworkSubmissions { get; set; } = [];
    public ICollection<PortfolioAlbum> PortfolioAlbums { get; set; } = [];
    public ICollection<ChatMessage> SentMessages { get; set; } = [];
    public ICollection<ChatMessage> ReceivedMessages { get; set; } = [];
  }
}
