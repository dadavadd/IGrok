using IGrok.Models;
using Microsoft.EntityFrameworkCore;

namespace IGrok.Application;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<Config> Configs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Key)
            .IsUnique();
    }
}
