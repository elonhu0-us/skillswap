using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Data;
using SkillSwap.Api.Models;
using SkillSwap.Api.Services.Location;

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILocationCacheService _cache;

    public LocationController(AppDbContext context, ILocationCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateLocation([FromBody] LocationRequest req)
    {
        var userId = GetUserIdFromJwt();
        if (userId == null)
            return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return NotFound();

        // Update user DB for persistence
        user.Latitude = req.Lat;
        user.Longitude = req.Lng;
        user.LocationUpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Write to Redis cache
        await _cache.SetLocationAsync(userId.Value, req.Lat, req.Lng);

        return Ok(new { message = "Location updated" });
    }

    private int? GetUserIdFromJwt()
    {
        if (User?.Claims == null) return null;

        var claim = User.Claims.FirstOrDefault(c => c.Type == "id");
        return claim != null ? int.Parse(claim.Value) : null;
    }
}

public class LocationRequest
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}
