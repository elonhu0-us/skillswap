using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Controllers;
using SkillSwap.Api.Data;
using SkillSwap.Api.Models;
using SkillSwap.Api.Services.Location;
using SkillSwap.Api.Services.Skill;
using Xunit;

public class SkillsControllerTests
{
    private AppDbContext GetDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    private SkillsController CreateController(AppDbContext db, ILocationCacheService cache)
    {
        var skillService = new SkillService(db);
        return new SkillsController(skillService, cache);
    }

    [Fact]
    public async Task GetByRadius_ReturnsNearbySkillOnly()
    {
        var db = GetDb();

        // create two users: user1 near, user2 far
        db.Users.Add(new User { Id = 1, Latitude = 40.0, Longitude = -86.0 });
        db.Users.Add(new User { Id = 2, Latitude = 80.0, Longitude = -10.0 });

        // skill owned by user1 and user2
        db.SkillPosts.Add(new SkillPost { Id = 1, Title = "Guitar", OwnerId = 1 });
        db.SkillPosts.Add(new SkillPost { Id = 2, Title = "Piano", OwnerId = 2 });

        await db.SaveChangesAsync();

        var fakeCache = new FakeLocationCacheService();
        await fakeCache.SetLocationAsync(99, 40.0, -86.0); // pretend requester at 40,-86

        var controller = CreateController(db, fakeCache);

        var ok = await controller.GetByRadius(userId: 99, radius: 50) as OkObjectResult;
        Assert.NotNull(ok);

        var list = ok.Value as IEnumerable<SkillPost>;
        Assert.Single(list);
        Assert.Contains(list, s => s.Title == "Guitar");
    }
}
