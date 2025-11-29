using FocusFrameAPI.Data;
using FocusFrameAPI.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FocusFrameTests.Helpers
{
  public class IntegrationTestBase : IDisposable
  {
    protected readonly FocusFrameDbContext Context;

    public IntegrationTestBase()
    {
      var options = new DbContextOptionsBuilder<FocusFrameDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
      Context = new FocusFrameDbContext(options);
    }

    public void Dispose()
    {
      Context.Dispose();
    }

    // Helper to simulate a logged-in user in the Controller
    protected void SetupUserContext(ControllerBase controller, int userId, UserRole role)
    {
      var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            };
      var identity = new ClaimsIdentity(claims, "TestAuth");
      var principal = new ClaimsPrincipal(identity);

      controller.ControllerContext = new ControllerContext
      {
        HttpContext = new DefaultHttpContext { User = principal }
      };
    }
  }
}
