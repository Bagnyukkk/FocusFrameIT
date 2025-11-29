using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class ChatThread : BaseEntity
  {
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime LastMessageAt { get; set; }
    public bool HasUnreadForAdmin { get; set; }
    public bool HasUnreadForUser { get; set; }

    public ICollection<ChatMessage> Messages { get; set; } = [];
  }
}