using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend.Core.Entities;

namespace Backend.Infrastructure.Data;

public class CarRentalDbContext : IdentityDbContext<ApplicationUser>
{
    public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Maintenance> Maintenances { get; set; }
    public DbSet<VehicleDamage> VehicleDamages { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Important pour Identity!

        // ApplicationUser configuration
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.DriverLicenseNumber).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            
            // Unique index on DriverLicenseNumber (if not null)
            entity.HasIndex(e => e.DriverLicenseNumber).IsUnique().HasFilter("[DriverLicenseNumber] IS NOT NULL");
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.HasIndex(e => e.Name).IsUnique();
            
            // Relationship with Vehicles
            entity.HasMany(e => e.Vehicles)
                .WithOne(v => v.Category)
                .HasForeignKey(v => v.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Vehicle configuration
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Brand).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.RegistrationNumber).IsUnique();
            entity.Property(e => e.DailyRate).HasColumnType("decimal(18,2)");
        });

        // Rental configuration - Updated to use ApplicationUser
        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");
            
            // Changed from Customer to ApplicationUser
            entity.HasOne(e => e.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.Rentals)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Rental)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(e => e.RentalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Maintenance configuration
        modelBuilder.Entity<Maintenance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cost).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.MaintenanceRecords)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // VehicleDamage configuration
        modelBuilder.Entity<VehicleDamage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RepairCost).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.DamageRecords)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Rental)
                .WithMany()
                .HasForeignKey(e => e.RentalId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories first
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Economy",
                Description = "Fuel-efficient and budget-friendly vehicles perfect for city driving",
                IsActive = true,
                DisplayOrder = 1,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = 2,
                Name = "Compact",
                Description = "Small to medium-sized vehicles that are easy to park and maneuver",
                IsActive = true,
                DisplayOrder = 2,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = 3,
                Name = "Midsize",
                Description = "Comfortable sedans with ample space for passengers and luggage",
                IsActive = true,
                DisplayOrder = 3,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = 4,
                Name = "SUV",
                Description = "Spacious sport utility vehicles ideal for families and adventures",
                IsActive = true,
                DisplayOrder = 4,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = 5,
                Name = "Luxury",
                Description = "Premium vehicles with top-tier comfort and advanced features",
                IsActive = true,
                DisplayOrder = 5,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = 6,
                Name = "Van",
                Description = "Large capacity vehicles perfect for group travel or cargo",
                IsActive = true,
                DisplayOrder = 6,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Category
            {
                Id = 7,
                Name = "Sports",
                Description = "High-performance vehicles for an exhilarating driving experience",
                IsActive = true,
                DisplayOrder = 7,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed Vehicles with CategoryId references
        modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle
            {
                Id = 1,
                Brand = "Toyota",
                Model = "Corolla",
                RegistrationNumber = "ABC123",
                Year = 2023,
                CategoryId = 2, // Compact
                DailyRate = 35.00m,
                Status = VehicleStatus.Available,
                Mileage = 16000,
                FuelType = "Gasoline",
                SeatingCapacity = 5
            },
            new Vehicle
            {
                Id = 2,
                Brand = "BMW",
                Model = "X5",
                RegistrationNumber = "XYZ789",
                Year = 2024,
                CategoryId = 4, // SUV
                DailyRate = 85.00m,
                Status = VehicleStatus.Available,
                Mileage = 5000,
                FuelType = "Diesel",
                SeatingCapacity = 7
            },
            new Vehicle
            {
                Id = 3,
                Brand = "Mercedes-Benz",
                Model = "C-Class",
                RegistrationNumber = "LUX456",
                Year = 2024,
                CategoryId = 5, // Luxury
                DailyRate = 120.00m,
                Status = VehicleStatus.Available,
                Mileage = 3000,
                FuelType = "Gasoline",
                SeatingCapacity = 5
            },
            new Vehicle
            {
                Id = 4,
                Brand = "Honda",
                Model = "Civic",
                RegistrationNumber = "ECO789",
                Year = 2023,
                CategoryId = 1, // Economy
                DailyRate = 28.00m,
                Status = VehicleStatus.Available,
                Mileage = 20000,
                FuelType = "Gasoline",
                SeatingCapacity = 5
            }
        );

        // Note: Customer seed data removed - users are managed through Identity
        // Sample customer will be created through registration

        // Seed vehicle history data for testing
        modelBuilder.SeedVehicleHistory();
    }
}
