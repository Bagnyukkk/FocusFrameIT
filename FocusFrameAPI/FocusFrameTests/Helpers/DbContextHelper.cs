using FocusFrameAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FocusFrameTests.Helpers
{
  public static class DbContextHelper
  {
    public static FocusFrameDbContext GetInMemoryDbContext()
    {
      var options = new DbContextOptionsBuilder<FocusFrameDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      return new FocusFrameDbContext(options);
    }
  }
}
