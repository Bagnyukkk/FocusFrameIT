using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class ForumComment : BaseEntity
  {

    [ForeignKey("ForumTopic")]
    public int TopicId { get; set; }
    public ForumTopic Topic { get; set; } = null!;

    [ForeignKey("User")]
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
  }
}
