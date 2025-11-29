using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class ForumTopic : BaseEntity
  {

    [ForeignKey("User")]
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }

    public ICollection<ForumComment> Comments { get; set; } = [];
  }
}
