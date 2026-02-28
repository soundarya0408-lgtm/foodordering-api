using FoodOrdering.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Infrastructure.Persistence;

public class FoodOrderingDbContext : DbContext
{
    public FoodOrderingDbContext(DbContextOptions<FoodOrderingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(builder =>
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.MobileNumber)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(c => c.Email)
                   .HasMaxLength(200);

            builder.Property(c => c.Address)
                   .HasMaxLength(500);
        });
    }
}
