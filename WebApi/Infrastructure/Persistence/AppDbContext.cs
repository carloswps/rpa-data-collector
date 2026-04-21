using Microsoft.EntityFrameworkCore;
using rpa_data_collector.Domain.Entities;

namespace rpa_data_collector.Infrastructure.Persistence;

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
            entity.Property(e =>
                e.Value).HasColumnType("decimal(18,4)");
            entity.Property(e =>
                e.Coin).HasMaxLength(20);
            entity.Property(e =>
                e.FontUrl).HasMaxLength(500);
        });
    }
}