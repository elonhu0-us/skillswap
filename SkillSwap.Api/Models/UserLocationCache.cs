namespace SkillSwap.Api.Models
{
    public class UserLocationCache
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
