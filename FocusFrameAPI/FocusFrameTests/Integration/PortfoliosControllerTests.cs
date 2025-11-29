using FocusFrameAPI.Controllers;
using FocusFrameAPI.Dtos.Portfolio;
using FocusFrameAPI.Services;
using FocusFrameTests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FocusFrameTests.Integration
{
  public class PortfoliosControllerTests : IntegrationTestBase
  {
    [Fact]
    public async Task CreateAlbum_Valid_ReturnsCreated()
    {
      // Arrange
      var mockEnv = new Mock<IWebHostEnvironment>();
      mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

      var service = new PortfolioService(Context, mockEnv.Object);
      var controller = new PortfoliosController(service);
      SetupUserContext(controller, 1, FocusFrameAPI.Enums.UserRole.Student);

      var dto = new CreateAlbumDto { Title = "My Art" };

      // Act
      var result = await controller.CreateAlbum(dto);

      // Assert
      var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
      var album = Assert.IsType<PortfolioAlbumDto>(createdResult.Value);
      Assert.Equal("My Art", album.Title);
    }
  }
}
