using SkillSwap.Api.Models;

namespace SkillSwap.Api.Services.Skill
{
    public interface ISkillService
    {
        Task<IEnumerable<SkillPost>> GetNearbySkillsAsync(double userLat, double userLng, double radiusMiles);
        Task<IEnumerable<SkillPost>> SearchSkillsAsync(string query);
        Task<SkillPost> CreateSkillAsync(SkillPost post);
    }
}
