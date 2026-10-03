namespace Nop.Modern.Api.Data;

using Microsoft.EntityFrameworkCore;
using Nop.Modern.Api.Models;

public class NopDbContext : DbContext
{
    public NopDbContext(DbContextOptions<NopDbContext> options) : base(options)
    {
    }

    public DbSet<ProductEntity> Products => Set<ProductEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.ToTable("Product");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(400);
            entity.Property(e => e.Sku).HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 4);
            entity.Property(e => e.OldPrice).HasPrecision(18, 4);
            entity.Property(e => e.ProductCost).HasPrecision(18, 4);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Deleted);
        });
    }
}
