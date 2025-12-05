using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Controllers;
using SkillSwap.Api.Data;
using SkillSwap.Api.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class LocationControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
            .Options;

        return new AppDbContext(options);
    }


    private LocationController GetControllerWithUser(AppDbContext context, int userId)
    {
        var controller = new LocationController(context);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("id", userId.ToString())
        }, "mock"));

        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        return controller;
    }

    [Fact]
    public async Task UpdateLocation_ShouldReturnUnauthorized_WhenUserMissing()
    {
        var context = GetDbContext();
        var controller = new LocationController(context);

        var result = await controller.UpdateLocation(new LocationRequest { Lat = 10, Lng = 20 });

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task UpdateLocation_ShouldReturnNotFound_WhenUserNotInDb()
    {
        var context = GetDbContext();
        var controller = GetControllerWithUser(context, userId: 999);

        var result = await controller.UpdateLocation(new LocationRequest { Lat = 10, Lng = 20 });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateLocation_ShouldUpdateUserLocation()
    {
        var context = GetDbContext();

        // Seed DB
        context.Users.Add(new User
        {
            Id = 1,
            Latitude = 0,
            Longitude = 0
        });

        await context.SaveChangesAsync();

        var controller = GetControllerWithUser(context, userId: 1);

        var req = new LocationRequest { Lat = 39.0, Lng = -86.0 };

        var result = await controller.UpdateLocation(req);

        Assert.IsType<OkObjectResult>(result);

        var updatedUser = await context.Users.FirstAsync(u => u.Id == 1);

        Assert.Equal(39.0, updatedUser.Latitude);
        Assert.Equal(-86.0, updatedUser.Longitude);

        Assert.True(updatedUser.LocationUpdatedAt.HasValue);
    }


    [Fact]
    public async Task LocationCache_ShouldStoreValue_WhenUpdatingLocation()
    {
        var context = GetDbContext();

        // Only add user — DO NOT insert a location cache manually
        context.Users.Add(new User { Id = 1 });
        await context.SaveChangesAsync();

        var controller = GetControllerWithUser(context, userId: 1);

        var req = new LocationRequest { Lat = 39.5, Lng = -86.2 };
        await controller.UpdateLocation(req);

        // Controller should create the cache automatically
        var cache = await context.UserLocationCaches
            .FirstOrDefaultAsync(c => c.UserId == 1);

        Assert.NotNull(cache);
        Assert.Equal(39.5, cache.Latitude);
        Assert.Equal(-86.2, cache.Longitude);
    }

}
