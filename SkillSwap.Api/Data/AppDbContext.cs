using Microsoft.EntityFrameworkCore;
using SkillSwap.Api.Models; // adjust if your namespace is different

namespace SkillSwap.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SkillPost> SkillPosts { get; set; }
    
    }
}
