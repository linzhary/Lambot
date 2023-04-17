using Lambot.Template.Plugins.FastLearning.Entity;
using Microsoft.EntityFrameworkCore;

namespace Lambot.Template.Plugins.FastLearning;

public class FastLearningDbContext : DbContext
{
    public DbSet<FastLearningRecord> Records { get; set; }

    public FastLearningDbContext(DbContextOptions<FastLearningDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}