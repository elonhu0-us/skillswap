namespace SkillSwap.Api.Models
{
    public class SkillPost
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = ""; // "offer" or "request"
    }
}
