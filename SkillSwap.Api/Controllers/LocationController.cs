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
        if (userId == null) return Unauthorized();

        var existingLoc = await _context.UserLocationCaches
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (existingLoc == null)
        {
            _context.UserLocationCaches.Add(new UserLocationCache
            {
                UserId = userId.Value,
                Latitude = req.Lat,
                Longitude = req.Lng,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existingLoc.Latitude = req.Lat;
            existingLoc.Longitude = req.Lng;
            existingLoc.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Location updated" });
    }

    private int? GetUserIdFromJwt()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "id");
        return claim != null ? int.Parse(claim.Value) : null;
    }
}

public class LocationRequest
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}
