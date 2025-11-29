using FocusFrameAPI.Dtos.Portfolio;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface IPortfolioService
  {
    Task<List<PortfolioAlbumDto>> GetUserAlbumsAsync(int userId);
    Task<PortfolioAlbumDto?> GetAlbumAsync(int albumId, int userId);
    Task<PortfolioAlbumDto> CreateAlbumAsync(int userId, CreateAlbumDto dto);
    Task<bool> UpdateAlbumAsync(int albumId, int userId, UpdateAlbumDto dto);
    Task<bool> DeleteAlbumAsync(int albumId, int userId);
    Task<PortfolioPhotoDto> AddPhotoAsync(int albumId, int userId, IFormFile file);
    Task<bool> DeletePhotoAsync(int photoId, int userId);
    Task<List<PortfolioPhotoDto>> GetDashboardPreviewAsync(int userId);
  }
}
