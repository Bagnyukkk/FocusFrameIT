using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class PortfolioAlbum : BaseEntity
  {

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }

    public ICollection<PortfolioPhoto> Photos { get; set; } = [];
  }
}
