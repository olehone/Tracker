using Microsoft.EntityFrameworkCore;
using Tracker.Domain.Entities;

namespace Tracker.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) 
    { 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(cfg =>
        {
            cfg.ToTable("Users");
            cfg.HasKey(u => u.Id);
            cfg.Property(u => u.Email).IsRequired();
            cfg.Property(u => u.Username).IsRequired();
        });
    }
}
