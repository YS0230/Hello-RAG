using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<SystemRule> SystemRules => Set<SystemRule>();
    public DbSet<JiebaDictEntry> JiebaDictEntries => Set<JiebaDictEntry>();
    public DbSet<UserActionLog> UserActionLogs => Set<UserActionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.Entity<SystemRule>()
            .Property(r => r.Embedding)
            .HasColumnType("vector(3072)");
    }
}
