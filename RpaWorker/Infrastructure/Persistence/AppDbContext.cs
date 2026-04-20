using Microsoft.EntityFrameworkCore;
using RpaWorker.Domain.Entities;

namespace RpaWorker.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Prices = Set<Price>();
    }

    public DbSet<Price> Prices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Price>(entity =>
        {
            entity.ToTable("prices");
            entity.HasKey(e => e.Id);
            entity.Property(e =>
                e.Value).HasColumnType("decimal(18,4)");
            entity.Property(e =>
                e.Coin).HasMaxLength(20);
            entity.Property(e =>
                e.FontUrl).HasMaxLength(500);
        });
    }
}