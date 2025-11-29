namespace FocusFrameAPI.Dtos.Portfolio
{
  // Album DTOs
  public class PortfolioAlbumDto
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PortfolioPhotoDto> Photos { get; set; } = new();
  }

  public class CreateAlbumDto
  {
    public string Title { get; set; } = string.Empty;
  }

  public class UpdateAlbumDto
  {
    public string Title { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
  }

  // Photo DTOs
  public class PortfolioPhotoDto
  {
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
  }

  public class PhotoUploadDto
  {
    public IFormFile File { get; set; } = null!;
  }
}
