using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Data;
using SkillSwap.Api.Models;

namespace SkillSwap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _db.Users.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(User u)
        {
            _db.Users.Add(u);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = u.Id }, u);
        }
    }
}
