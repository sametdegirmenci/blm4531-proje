using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Vehicle> Vehicles { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Role)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        });

        // Configure Vehicle entity
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId);

            entity.Property(e => e.Brand)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Model)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Year)
                .IsRequired();

            entity.Property(e => e.PricePerDay)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20);

            entity.HasIndex(e => e.LicensePlate)
                .IsUnique();

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // Configure Reservation entity
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId);

            entity.Property(e => e.StartDate)
                .IsRequired();

            entity.Property(e => e.EndDate)
                .IsRequired();

            entity.Property(e => e.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt);

            // Configure relationships
            entity.HasOne(e => e.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.Reservations)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Use a static date for seed data (not DateTime.UtcNow which changes)
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Pre-computed BCrypt hash for "Admin123!" to avoid non-deterministic behavior
        // This hash was generated using BCrypt.Net.BCrypt.HashPassword("Admin123!")
        var adminPasswordHash = "$2a$11$i6OXqbrcPOiX/vukuoFSwOXg1t4xkZabCIsEMnYgMWPu0YpwoPc1O";

        // Seed admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                FullName = "Samet Admin",
                Email = "sametadmin@example.com",
                PasswordHash = adminPasswordHash,
                Role = UserRole.Admin,
                CreatedAt = seedDate,
                IsActive = true
            }
        );

        // Seed sample vehicles
        modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle
            {
                VehicleId = 1,
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2023,
                PricePerDay = 50.00m,
                IsAvailable = true,
                LicensePlate = "34ABC123",
                ImageUrl = "https://example.com/images/toyota-corolla.jpg",
                CreatedAt = seedDate
            },
            new Vehicle
            {
                VehicleId = 2,
                Brand = "Honda",
                Model = "Civic",
                Year = 2023,
                PricePerDay = 55.00m,
                IsAvailable = true,
                LicensePlate = "34DEF456",
                ImageUrl = "https://example.com/images/honda-civic.jpg",
                CreatedAt = seedDate
            },
            new Vehicle
            {
                VehicleId = 3,
                Brand = "BMW",
                Model = "3 Series",
                Year = 2024,
                PricePerDay = 100.00m,
                IsAvailable = true,
                LicensePlate = "34GHI789",
                ImageUrl = "https://example.com/images/bmw-3series.jpg",
                CreatedAt = seedDate
            }
        );
    }
}
