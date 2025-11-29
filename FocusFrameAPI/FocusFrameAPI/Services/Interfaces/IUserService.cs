using FocusFrameAPI.Dtos.User;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface IUserService
  {
    Task DeleteUserAsync(int userId);
    Task<IEnumerable<UserProfileDto>> GetAllStudentsAsync();
    Task<UserProfileDto> GetUserProfileAsync(int userId);
    Task<UserProfileDto> UpdateUserProfileAsync(int userId, UpdateProfileDto dto);
  }
}
