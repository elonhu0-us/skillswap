using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Data;
using SkillSwap.Api.Models;

namespace SkillSwap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SkillsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/skills?userLat=0&userLng=0&radiusMiles=50
        // Currently does NOT filter by distance (future-use)
        [HttpGet]
        public async Task<IActionResult> GetSkills(
            [FromQuery] double userLat,
            [FromQuery] double userLng,
            [FromQuery] double radiusMiles = 50)
        {
            var skills = await _db.SkillPosts.ToListAsync();
            return Ok(skills);
        }

        // GET: api/skills/search?query=plumbing
        [HttpGet("search")]
        public async Task<IActionResult> SearchSkills([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query required.");

            var results = await _db.SkillPosts
                .Where(s =>
                    s.Title.Contains(query) ||
                    s.Description.Contains(query) ||
                    s.Type.Contains(query)
                )
                .ToListAsync();

            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSkill([FromBody] SkillPost post)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _db.SkillPosts.Add(post);
            await _db.SaveChangesAsync();

            return Ok(post);
        }
    }
}