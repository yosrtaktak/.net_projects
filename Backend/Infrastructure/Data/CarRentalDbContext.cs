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
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Maintenance> Maintenances { get; set; }
    public DbSet<VehicleDamage> VehicleDamages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Important pour Identity!

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

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.DriverLicenseNumber).IsUnique();
        });

        // Rental configuration
        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Rentals)
                .HasForeignKey(e => e.CustomerId)
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
        // Seed Vehicles
        modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle
            {
                Id = 1,
                Brand = "Toyota",
                Model = "Corolla",
                RegistrationNumber = "ABC123",
                Year = 2023,
                Category = VehicleCategory.Compact,
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
                Category = VehicleCategory.SUV,
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
                Category = VehicleCategory.Luxury,
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
                Category = VehicleCategory.Economy,
                DailyRate = 28.00m,
                Status = VehicleStatus.Available,
                Mileage = 20000,
                FuelType = "Gasoline",
                SeatingCapacity = 5
            }
        );

        // Seed Sample Customer
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                DriverLicenseNumber = "DL123456",
                DateOfBirth = new DateTime(1990, 5, 15),
                Address = "123 Main St, City, Country",
                RegistrationDate = new DateTime(2024, 1, 1),
                Tier = CustomerTier.Gold
            }
        );

        // Seed vehicle history data for testing
        modelBuilder.SeedVehicleHistory();
    }
}
