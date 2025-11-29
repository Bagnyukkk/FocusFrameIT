using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Portfolio;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameAPI.Services
{
  public class PortfolioService : IPortfolioService
  {
    private readonly FocusFrameDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public PortfolioService(FocusFrameDbContext context, IWebHostEnvironment environment)
    {
      _context = context;
      _environment = environment;
    }

    public async Task<List<PortfolioAlbumDto>> GetUserAlbumsAsync(int userId)
    {
      var albums = await _context.PortfolioAlbums
          .Where(a => a.UserId == userId)
          .Include(a => a.Photos)
          .OrderByDescending(a => a.CreatedAt)
          .ToListAsync();

      return albums.Select(MapToAlbumDto).ToList();
    }

    public async Task<PortfolioAlbumDto?> GetAlbumAsync(int albumId, int userId)
    {
      var album = await _context.PortfolioAlbums
          .Include(a => a.Photos)
          .FirstOrDefaultAsync(a => a.Id == albumId && a.UserId == userId);

      return album == null ? null : MapToAlbumDto(album);
    }

    public async Task<PortfolioAlbumDto> CreateAlbumAsync(int userId, CreateAlbumDto dto)
    {
      var album = new PortfolioAlbum
      {
        UserId = userId,
        Title = dto.Title,
        IsFavorite = false,
        Photos = new List<PortfolioPhoto>() // [cite: 207, 208]
      };

      _context.PortfolioAlbums.Add(album);
      await _context.SaveChangesAsync();

      return MapToAlbumDto(album);
    }

    public async Task<bool> UpdateAlbumAsync(int albumId, int userId, UpdateAlbumDto dto)
    {
      var album = await _context.PortfolioAlbums
          .FirstOrDefaultAsync(a => a.Id == albumId && a.UserId == userId);

      if (album == null) return false;

      album.Title = dto.Title;
      album.IsFavorite = dto.IsFavorite; // [cite: 207]

      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> DeleteAlbumAsync(int albumId, int userId)
    {
      var album = await _context.PortfolioAlbums
          .FirstOrDefaultAsync(a => a.Id == albumId && a.UserId == userId);

      if (album == null) return false;

      _context.PortfolioAlbums.Remove(album);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<PortfolioPhotoDto> AddPhotoAsync(int albumId, int userId, IFormFile file)
    {
      // Verify album ownership
      var album = await _context.PortfolioAlbums
          .FirstOrDefaultAsync(a => a.Id == albumId && a.UserId == userId);

      if (album == null) throw new KeyNotFoundException("Album not found or access denied.");

      // 1. Validation: Max 500KB 
      const long maxFileSize = 500 * 1024;
      if (file.Length > maxFileSize)
      {
        throw new ArgumentException("File size exceeds the 500KB limit.");
      }

      // 2. File Storage Logic (Local Storage Simulation)
      // In production, this would upload to S3/Azure Blob Storage
      var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "portfolio");
      if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

      var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
      var filePath = Path.Combine(uploadsFolder, uniqueFileName);

      using (var fileStream = new FileStream(filePath, FileMode.Create))
      {
        await file.CopyToAsync(fileStream);
      }

      // 3. Save to DB [cite: 209, 210]
      var photo = new PortfolioPhoto
      {
        AlbumId = albumId,
        ImageUrl = $"/uploads/portfolio/{uniqueFileName}",
        FileSizeBytes = file.Length
      };

      _context.PortfolioPhotos.Add(photo);
      await _context.SaveChangesAsync();

      return MapToPhotoDto(photo);
    }

    public async Task<bool> DeletePhotoAsync(int photoId, int userId)
    {
      // Verify photo and ownership via Album
      var photo = await _context.PortfolioPhotos
          .Include(p => p.Album)
          .FirstOrDefaultAsync(p => p.Id == photoId && p.Album.UserId == userId);

      if (photo == null) return false;

      // Optional: Delete physical file here

      _context.PortfolioPhotos.Remove(photo);
      await _context.SaveChangesAsync();
      return true;
    }

    // Implementation of Dashboard Preview requirement 
    public async Task<List<PortfolioPhotoDto>> GetDashboardPreviewAsync(int userId)
    {
      var photos = await _context.PortfolioPhotos
          .Where(p => p.Album.UserId == userId)
          // CHANGE THIS LINE:
          .OrderByDescending(p => p.CreatedAt) // Must be Descending to get the "Last" photos first
          .Take(6)
          .ToListAsync();

      return photos.Select(MapToPhotoDto).ToList();
    }

    // Helpers
    private static PortfolioAlbumDto MapToAlbumDto(PortfolioAlbum album)
    {
      return new PortfolioAlbumDto
      {
        Id = album.Id,
        Title = album.Title,
        IsFavorite = album.IsFavorite,
        CreatedAt = album.CreatedAt,
        Photos = album.Photos?.Select(MapToPhotoDto).ToList() ?? new List<PortfolioPhotoDto>()
      };
    }

    private static PortfolioPhotoDto MapToPhotoDto(PortfolioPhoto photo)
    {
      return new PortfolioPhotoDto
      {
        Id = photo.Id,
        ImageUrl = photo.ImageUrl,
        FileSizeBytes = photo.FileSizeBytes,
        CreatedAt = photo.CreatedAt
      };
    }
  }
}
