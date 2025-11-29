using FocusFrameAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class Notification : BaseEntity
  {
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.System;

    public int? ReferenceId { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
  }
}
