using ApplicationInsightsRepro.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicationInsightsRepro.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OrderDate).IsRequired();
            entity.Property(e => e.TotalAmount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.OrderNumber).IsUnique();

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
