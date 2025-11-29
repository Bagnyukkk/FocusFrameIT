using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class ChatMessage : BaseEntity
  {

    [ForeignKey("User")]
    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;
    public string MessageText { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }

    [ForeignKey(nameof(Thread))]
    public int ThreadId { get; set; }
    public ChatThread Thread { get; set; } = null!;

    public bool IsRead { get; set; }
  }
}
