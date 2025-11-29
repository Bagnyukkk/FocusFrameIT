using FocusFrameAPI.Data;
using FocusFrameAPI.Dtos.Auth;
using FocusFrameAPI.Entities;
using FocusFrameAPI.Enums;
using FocusFrameAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FocusFrameAPI.Services
{
  public class AuthService : IAuthService
  {
    private readonly FocusFrameDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(FocusFrameDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto request)
    {
      if (await _context.Users.AnyAsync(u => u.Email == request.Email))
      {
        throw new Exception("User with this email already exists.");
      }

      // 2. Hash the password
      string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

      // 3. Create User Entity
      var user = new User
      {
        Email = request.Email,
        PasswordHash = passwordHash, 
        FullName = request.FullName,
        Role = UserRole.Student,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      // 4. Return Token
      return GenerateToken(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto request)
    {
      // 1. Find User
      var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Email == request.Email);

      if (user == null)
      {
        throw new Exception("Invalid email or password.");
      }

      // 2. Verify Password
      if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
      {
        throw new Exception("Invalid email or password.");
      }

      // 3. Generate Token
      return GenerateToken(user);
    }

    private AuthResponseDto GenerateToken(User user)
    {
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString())
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
          _configuration.GetSection("JwtSettings:SecretKey").Value!));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(
              double.Parse(_configuration.GetSection("JwtSettings:DurationInMinutes").Value!)),
        SigningCredentials = creds,
        Issuer = _configuration.GetSection("JwtSettings:Issuer").Value,
        Audience = _configuration.GetSection("JwtSettings:Audience").Value
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);

      return new AuthResponseDto
      {
        Token = tokenHandler.WriteToken(token),
        Email = user.Email,
        Role = user.Role.ToString()
      };
    }
  }
}
