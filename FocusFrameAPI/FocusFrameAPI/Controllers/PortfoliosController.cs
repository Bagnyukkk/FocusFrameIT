using FocusFrameAPI.Dtos.Portfolio;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FocusFrameAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class PortfoliosController : ControllerBase
  {
    private readonly IPortfolioService _portfolioService;

    public PortfoliosController(IPortfolioService portfolioService)
    {
      _portfolioService = portfolioService;
    }

    // Helper to get current user ID
    private int GetCurrentUserId()
    {
      var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
      if (idClaim == null || !int.TryParse(idClaim.Value, out int userId))
      {
        throw new UnauthorizedAccessException("Invalid user token.");
      }
      return userId;
    }

    [HttpGet("albums")]
    public async Task<ActionResult<List<PortfolioAlbumDto>>> GetAlbums()
    {
      var albums = await _portfolioService.GetUserAlbumsAsync(GetCurrentUserId());
      return Ok(albums);
    }

    [HttpGet("albums/{id}")]
    public async Task<ActionResult<PortfolioAlbumDto>> GetAlbum(int id)
    {
      var album = await _portfolioService.GetAlbumAsync(id, GetCurrentUserId());
      if (album == null) return NotFound();
      return Ok(album);
    }

    [HttpPost("albums")]
    public async Task<ActionResult<PortfolioAlbumDto>> CreateAlbum([FromBody] CreateAlbumDto dto)
    {
      var album = await _portfolioService.CreateAlbumAsync(GetCurrentUserId(), dto);
      return CreatedAtAction(nameof(GetAlbum), new { id = album.Id }, album);
    }

    [HttpPut("albums/{id}")]
    public async Task<IActionResult> UpdateAlbum(int id, [FromBody] UpdateAlbumDto dto)
    {
      var updated = await _portfolioService.UpdateAlbumAsync(id, GetCurrentUserId(), dto);
      if (!updated) return NotFound();
      return NoContent();
    }

    [HttpDelete("albums/{id}")]
    public async Task<IActionResult> DeleteAlbum(int id)
    {
      var deleted = await _portfolioService.DeleteAlbumAsync(id, GetCurrentUserId());
      if (!deleted) return NotFound();
      return NoContent();
    }

    // POST api/portfolios/albums/5/photos
    [HttpPost("albums/{albumId}/photos")]
    public async Task<ActionResult<PortfolioPhotoDto>> UploadPhoto(int albumId, IFormFile file)
    {
      if (file == null || file.Length == 0)
        return BadRequest("No file uploaded.");

      try
      {
        var photo = await _portfolioService.AddPhotoAsync(albumId, GetCurrentUserId(), file);
        return Ok(photo);
      }
      catch (KeyNotFoundException)
      {
        return NotFound("Album not found.");
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ex.Message); // Returns error if > 500KB
      }
    }

    [HttpDelete("photos/{id}")]
    public async Task<IActionResult> DeletePhoto(int id)
    {
      var deleted = await _portfolioService.DeletePhotoAsync(id, GetCurrentUserId());
      if (!deleted) return NotFound();
      return NoContent();
    }

    // Endpoint for Dashboard "My Account" page
    [HttpGet("dashboard-preview")]
    public async Task<ActionResult<List<PortfolioPhotoDto>>> GetDashboardPreview()
    {
      var photos = await _portfolioService.GetDashboardPreviewAsync(GetCurrentUserId());
      return Ok(photos);
    }
  }
}
