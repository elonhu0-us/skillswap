using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Controllers;
using SkillSwap.Api.Data;
using SkillSwap.Api.Models;
using SkillSwap.Api.Services.Location;
using System.Security.Claims;
using Xunit;


public class LocationControllerTests
{
    private AppDbContext GetDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private HttpContext FakeHttpContextWithUser(int userId)
    {
        var ctx = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("id", userId.ToString())
        });

        ctx.User = new ClaimsPrincipal(identity);
        return ctx;
    }

    [Fact]
    public async Task UpdateLocation_ShouldWriteToRedisCache()
    {
        // Arrange
        var db = GetDb();
        db.Users.Add(new User { Id = 1 });
        await db.SaveChangesAsync();

        var redis = new FakeLocationCacheService();

        var controller = new LocationController(db, redis)
        {
            ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = FakeHttpContextWithUser(1)
            }
        };

        var req = new LocationRequest { Lat = 39.2, Lng = -86.4 };

        // Act
        await controller.UpdateLocation(req);
        var stored = await redis.GetLocationAsync(1);

        // Assert
        Assert.NotNull(stored);
        Assert.Equal(39.2, stored.Value.lat);
        Assert.Equal(-86.4, stored.Value.lng);
    }
}
