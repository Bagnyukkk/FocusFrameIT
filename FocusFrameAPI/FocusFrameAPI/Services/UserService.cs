using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.User;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FocusFrameAPI.Services
{
  public class UserService : IUserService
  {
    private readonly FocusFrameDbContext _context;

    public UserService(FocusFrameDbContext context)
    {
      _context = context;
    }

    // Retrieves the profile for the currently logged-in user
    public async Task<UserProfileDto> GetUserProfileAsync(int userId)
    {
      var user = await _context.Users.FindAsync(userId);
      if (user == null) throw new KeyNotFoundException("User not found");

      return MapToDto(user);
    }

    public async Task<UserProfileDto> UpdateUserProfileAsync(int userId, UpdateProfileDto dto)
    {
      var user = await _context.Users.FindAsync(userId);
      if (user == null) throw new KeyNotFoundException("User not found");

      user.FullName = dto.FullName;
      user.Nickname = dto.Nickname;
      user.Country = dto.Country;

      await _context.SaveChangesAsync();

      return MapToDto(user);
    }

    public async Task<IEnumerable<UserProfileDto>> GetAllStudentsAsync()
    {
      var students = await _context.Users.Where(u => u.Role == UserRole.Student)
          .ToListAsync();

      return students.Select(MapToDto);
    }

    // Admin function: Delete a user
    public async Task DeleteUserAsync(int userId)
    {
      var user = await _context.Users.FindAsync(userId);
      if (user == null) throw new KeyNotFoundException("User not found");

      _context.Users.Remove(user);
      await _context.SaveChangesAsync();
    }

    // Helper to map Entity to DTO
    private static UserProfileDto MapToDto(User user)
    {
      return new UserProfileDto
      {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Nickname = user.Nickname,
        Country = user.Country,
        Role = user.Role.ToString()
      };
    }
  }
}
