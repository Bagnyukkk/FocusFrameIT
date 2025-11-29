using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusFrameAPI.Entities
{
  public class PortfolioPhoto : BaseEntity
  {
    [ForeignKey("PortfolioAlbum")]
    public int AlbumId { get; set; }
    public PortfolioAlbum Album { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
  }
}
