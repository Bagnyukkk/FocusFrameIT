using FocusFrameAPI.Controllers;
using FocusFrameAPI.Dtos.Auth;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FocusFrameTests.Integration
{
  public class AuthControllerTests
  {
    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
      // Arrange
      var mockService = new Mock<IAuthService>();
      var responseDto = new AuthResponseDto { Token = "jwt_token", Role = "Student" };

      mockService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
                 .ReturnsAsync(responseDto);

      var controller = new AuthController(mockService.Object);

      // Act
      var result = await controller.Login(new LoginDto { Email = "test", Password = "pass" });

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var data = Assert.IsType<AuthResponseDto>(okResult.Value);
      Assert.Equal("jwt_token", data.Token);
    }
  }
}
