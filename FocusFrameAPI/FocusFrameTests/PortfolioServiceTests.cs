using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Portfolio;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FocusFrameTests
{
  public class PortfolioServiceTests : IDisposable
  {
    private readonly DbContextOptions<FocusFrameDbContext> _dbOptions;
    private readonly Mock<IWebHostEnvironment> _mockEnvironment;
    private readonly string _tempPath;

    public PortfolioServiceTests()
    {
      // 1. Setup In-Memory Database with a unique name for each test
      _dbOptions = new DbContextOptionsBuilder<FocusFrameDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      // 2. Setup Mock Environment for File Uploads
      _mockEnvironment = new Mock<IWebHostEnvironment>();

      // Create a temporary directory for file upload tests to avoid cluttering the actual project
      _tempPath = Path.Combine(Path.GetTempPath(), "FocusFrameTests_" + Guid.NewGuid());
      Directory.CreateDirectory(_tempPath);
      _mockEnvironment.Setup(e => e.WebRootPath).Returns(_tempPath);
    }

    // Cleanup after tests (delete temp files)
    public void Dispose()
    {
      if (Directory.Exists(_tempPath))
      {
        Directory.Delete(_tempPath, true);
      }
    }

    // Helper to get a fresh context
    private FocusFrameDbContext GetContext() => new FocusFrameDbContext(_dbOptions);

    [Fact]
    public async Task CreateAlbumAsync_ShouldSaveAlbumToDatabase()
    {
      // Arrange
      var userId = 1;
      var dto = new CreateAlbumDto { Title = "Summer 2024" };

      using var context = GetContext();
      var service = new PortfolioService(context, _mockEnvironment.Object);

      // Act
      var result = await service.CreateAlbumAsync(userId, dto);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(dto.Title, result.Title);
      Assert.Equal(userId, context.PortfolioAlbums.First().UserId);
    }

    [Fact]
    public async Task AddPhotoAsync_ShouldThrowException_WhenFileExceeds500KB()
    {
      // Arrange
      var userId = 1;
      var albumId = 1;
      using var context = GetContext();

      // Seed Album
      context.PortfolioAlbums.Add(new PortfolioAlbum { Id = albumId, UserId = userId, Title = "Test" });
      await context.SaveChangesAsync();

      var service = new PortfolioService(context, _mockEnvironment.Object);

      // Mock a large file (501KB)
      var mockFile = new Mock<IFormFile>();
      var oversized = (500 * 1024) + 1;
      mockFile.Setup(f => f.Length).Returns(oversized);
      mockFile.Setup(f => f.FileName).Returns("large_image.jpg");

      // Act & Assert
      var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
          service.AddPhotoAsync(albumId, userId, mockFile.Object));

      Assert.Equal("File size exceeds the 500KB limit.", exception.Message);
    }

    [Fact]
    public async Task AddPhotoAsync_ShouldSaveFileAndEntity_WhenValid()
    {
      // Arrange
      var userId = 1;
      var albumId = 1;
      using var context = GetContext();

      context.PortfolioAlbums.Add(new PortfolioAlbum { Id = albumId, UserId = userId, Title = "Test" });
      await context.SaveChangesAsync();

      var service = new PortfolioService(context, _mockEnvironment.Object);

      // Mock valid file
      var mockFile = new Mock<IFormFile>();
      mockFile.Setup(f => f.Length).Returns(1024); // 1KB
      mockFile.Setup(f => f.FileName).Returns("test.jpg");

      // Mock CopyToAsync behavior (required because the service awaits it)
      mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
          .Returns(Task.CompletedTask);

      // Act
      var result = await service.AddPhotoAsync(albumId, userId, mockFile.Object);

      // Assert
      Assert.NotNull(result);
      Assert.StartsWith("/uploads/portfolio/", result.ImageUrl);

      // Verify DB persistence
      var savedPhoto = await context.PortfolioPhotos.FirstAsync();
      Assert.Equal(albumId, savedPhoto.AlbumId);
    }

    [Fact]
    public async Task GetDashboardPreviewAsync_ShouldReturnLast6Photos_OrderedByDateDesc()
    {
      // Arrange
      var userId = 1;
      using var context = GetContext();
      var service = new PortfolioService(context, _mockEnvironment.Object);

      // 1. Create Album
      var album = new PortfolioAlbum { Id = 1, UserId = userId, Title = "Preview Test" };
      context.PortfolioAlbums.Add(album);

      // 2. Create Photos (Don't set dates yet)
      var photos = new List<PortfolioPhoto>();
      for (int i = 1; i <= 10; i++)
      {
        photos.Add(new PortfolioPhoto
        {
          // Do not set Id manually if you want to simulate real DB behavior, 
          // but for In-Memory with explicit assignment it's fine.
          Id = i,
          AlbumId = 1,
          ImageUrl = $"img{i}.jpg"
        });
      }

      // 3. First Save: This triggers the DbContext override, setting CreatedAt = Now
      context.PortfolioPhotos.AddRange(photos);
      await context.SaveChangesAsync();

      // 4. Update Dates: Now we overwrite the CreatedAt. 
      // Since state is "Modified", DbContext won't overwrite CreatedAt again.
      foreach (var photo in photos)
      {
        photo.CreatedAt = DateTime.UtcNow.AddDays(-photo.Id); // Id 1 = Yesterday (Newest)
      }
      await context.SaveChangesAsync();

      // Act
      var result = await service.GetDashboardPreviewAsync(userId);

      // Assert
      Assert.Equal(6, result.Count);
      Assert.Equal(1, result[0].Id); // Newest (1 day ago)
      Assert.Equal(2, result[1].Id);
      Assert.Equal(6, result[5].Id);
      Assert.DoesNotContain(result, p => p.Id == 7);
    }
    [Fact]
    public async Task DeleteAlbum_ShouldRemoveAlbumAndPhotos()
    {
      // Arrange
      var userId = 1;
      using var context = GetContext();
      var service = new PortfolioService(context, _mockEnvironment.Object);

      var album = new PortfolioAlbum { Id = 1, UserId = userId, Title = "To Delete" };
      var photo = new PortfolioPhoto { Id = 1, AlbumId = 1, ImageUrl = "test.jpg" };

      context.PortfolioAlbums.Add(album);
      context.PortfolioPhotos.Add(photo);
      await context.SaveChangesAsync();

      // Act
      var result = await service.DeleteAlbumAsync(1, userId);

      // Assert
      Assert.True(result);
      Assert.Empty(context.PortfolioAlbums);
      Assert.Empty(context.PortfolioPhotos); // Cascade check (simulated in logic)
    }

    [Fact]
    public async Task UpdateAlbum_ShouldReturnFalse_WhenUserDoesNotOwnAlbum()
    {
      // Arrange
      var ownerId = 1;
      var hackerId = 2;
      using var context = GetContext();
      var service = new PortfolioService(context, _mockEnvironment.Object);

      context.PortfolioAlbums.Add(new PortfolioAlbum { Id = 1, UserId = ownerId, Title = "Private" });
      await context.SaveChangesAsync();

      // Act
      var result = await service.UpdateAlbumAsync(1, hackerId, new UpdateAlbumDto { Title = "Hacked" });

      // Assert
      Assert.False(result);
      var album = await context.PortfolioAlbums.FindAsync(1);
      Assert.Equal("Private", album!.Title); // Should verify title didn't change
    }
  }
}
