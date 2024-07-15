using Lambot.Template.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Lambot.Template.Database;

public sealed class LambotDbContext : DbContext
{
    public DbSet<FastLearningRecord> FastLearningRecords => Set<FastLearningRecord>();
    public DbSet<MessageHistory> MessageHistories => Set<MessageHistory>();
    public DbSet<AtMessageHistory> AtMessageHistories => Set<AtMessageHistory>();

    public LambotDbContext(DbContextOptions<LambotDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}