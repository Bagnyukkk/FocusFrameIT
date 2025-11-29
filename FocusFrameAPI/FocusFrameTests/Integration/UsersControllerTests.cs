using FocusFrameAPI.Controllers;
using FocusFrameAPI.Dtos.User;
using FocusFrameAPI.Services.Interfaces;
using FocusFrameTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FocusFrameTests.Integration
{
  public class UsersControllerTests : IntegrationTestBase
  {
    [Fact]
    public async Task GetMyProfile_Found_ReturnsOk()
    {
      // Arrange
      var mockService = new Mock<IUserService>();
      var profile = new UserProfileDto { FullName = "Test User", Email = "t@t.com" };

      mockService.Setup(s => s.GetUserProfileAsync(1))
                 .ReturnsAsync(profile);

      var controller = new UsersController(mockService.Object);
      SetupUserContext(controller, 1, FocusFrameAPI.Enums.UserRole.Student);

      // Act
      var result = await controller.GetMyProfile();

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result); // [cite: 416]
      var dto = Assert.IsType<UserProfileDto>(okResult.Value);
      Assert.Equal("Test User", dto.FullName);
    }

    [Fact]
    public async Task DeleteUser_Admin_ReturnsNoContent()
    {
      // Arrange
      var mockService = new Mock<IUserService>();
      mockService.Setup(s => s.DeleteUserAsync(5)).Returns(Task.CompletedTask);

      var controller = new UsersController(mockService.Object);
      SetupUserContext(controller, 99, FocusFrameAPI.Enums.UserRole.Admin);

      // Act
      var result = await controller.DeleteUser(5);

      // Assert
      Assert.IsType<NoContentResult>(result); // [cite: 421]
    }
  }
}
