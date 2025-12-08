using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Data;
using SkillSwap.Api.Models;

namespace SkillSwap.Api.Services.Skill
{
    public class SkillService : ISkillService
    {
        private readonly AppDbContext _db;
        public SkillService(AppDbContext db) { _db = db; }

        public async Task<IEnumerable<SkillPost>> GetNearbySkillsAsync(double userLat, double userLng, double radiusMiles)
        {
            var skills = await _db.SkillPosts.ToListAsync();
            var users = await _db.Users.ToListAsync();

            var res = from s in skills
                      join u in users on s.OwnerId equals u.Id
                      where u.Latitude.HasValue && u.Longitude.HasValue &&
                            DistanceMiles(userLat, userLng, u.Latitude.Value, u.Longitude.Value) <= radiusMiles
                      select s;

            return res;
        }

        public async Task<IEnumerable<SkillPost>> SearchSkillsAsync(string query)
        {
            return await _db.SkillPosts
                .Where(s => s.Title.Contains(query) || s.Description.Contains(query) || s.Type.Contains(query))
                .ToListAsync();
        }

        public async Task<SkillPost> CreateSkillAsync(SkillPost post)
        {
            _db.SkillPosts.Add(post);
            await _db.SaveChangesAsync();
            return post;
        }

        private double DistanceMiles(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 3958.8;
            var dLat = (lat2 - lat1) * Math.PI / 180.0;
            var dLon = (lon2 - lon1) * Math.PI / 180.0;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
