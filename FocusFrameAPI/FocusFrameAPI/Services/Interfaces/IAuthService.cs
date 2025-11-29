using FocusFrameAPI.Dtos.Auth;

namespace FocusFrameAPI.Services.Interfaces
{
  public interface IAuthService
  {
    Task<AuthResponseDto> LoginAsync(LoginDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterDto request);
  }
}
