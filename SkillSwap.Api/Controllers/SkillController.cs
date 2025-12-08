using Microsoft.AspNetCore.Mvc;
using SkillSwap.Api.Models;
using SkillSwap.Api.Services;
using SkillSwap.Api.Services.Skill;
using SkillSwap.Api.Services.Location;
namespace SkillSwap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;
        private readonly ILocationCacheService _locationCache;

        public SkillsController(ISkillService skillService, ILocationCacheService locationCache)
        {
            _skillService = skillService;
            _locationCache = locationCache;
        }

        // GET api/skills/radius?userId=1&radius=50
        [HttpGet("radius")]
        public async Task<IActionResult> GetByRadius([FromQuery] int userId, [FromQuery] double radius = 50)
        {
            var loc = await _locationCache.GetLocationAsync(userId);
            if (loc == null) return BadRequest("User location not found.");

            var skills = await _skillService.GetNearbySkillsAsync(loc.Value.lat, loc.Value.lng, radius);
            return Ok(skills);
        }

        // GET api/skills/search?query=xxx
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("query required");
            var res = await _skillService.SearchSkillsAsync(query);
            return Ok(res);
        }

        // POST api/skills
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SkillPost post)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _skillService.CreateSkillAsync(post);
            return Ok(created);
        }
    }
}
