using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Data;
using SkillSwap.Api.Models;

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly AppDbContext _context;

    public LocationController(AppDbContext context)
    {
        _context = context;
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

        // Update user entity
        user.Latitude = req.Lat;
        user.Longitude = req.Lng;
        user.LocationUpdatedAt = DateTime.UtcNow;

        // Update / insert cache
        var cache = await _context.UserLocationCaches
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (cache == null)
        {
            cache = new UserLocationCache
            {
                UserId = userId.Value,
                Latitude = req.Lat,
                Longitude = req.Lng,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserLocationCaches.Add(cache);
        }
        else
        {
            cache.Latitude = req.Lat;
            cache.Longitude = req.Lng;
            cache.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

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
