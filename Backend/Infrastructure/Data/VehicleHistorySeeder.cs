using Microsoft.EntityFrameworkCore;
using Backend.Core.Entities;
using Backend.Infrastructure.Data;

namespace Backend.Infrastructure.Data;

public static class VehicleHistorySeeder
{
    public static void SeedVehicleHistory(this ModelBuilder modelBuilder)
    {
        // NOTE: Seed data commented out due to Customer to ApplicationUser refactoring
        // This seed data relied on the old Customers table which has been removed
        // To re-enable, update with valid ApplicationUser IDs after migration
        
        /*
        var vehicleId = 1; // Toyota Corolla
        var userId = "user-guid-here"; // Replace with actual ApplicationUser GUID

        // Seed some past rentals for the Toyota Corolla
        modelBuilder.Entity<Rental>().HasData(
            new Rental
            {
                Id = 1,
                UserId = userId,
                VehicleId = vehicleId,
                StartDate = new DateTime(2024, 1, 15),
                EndDate = new DateTime(2024, 1, 20),
                ActualReturnDate = new DateTime(2024, 1, 20),
                TotalCost = 175.00m,
                Status = RentalStatus.Completed,
                StartMileage = 15000,
                EndMileage = 15250,
                Notes = "Regular rental, vehicle returned in good condition",
                CreatedAt = new DateTime(2024, 1, 10)
            }
            // ... more rentals
        );
        */

        // Seed maintenance records for Toyota Corolla
        modelBuilder.Entity<Maintenance>().HasData(
            new Maintenance
            {
                Id = 1,
                VehicleId = 1,
                ScheduledDate = new DateTime(2024, 1, 5),
                CompletedDate = new DateTime(2024, 1, 5),
                Description = "Regular oil change and filter replacement",
                Cost = 85.00m,
                Type = MaintenanceType.Routine,
                Status = MaintenanceStatus.Completed
            },
            new Maintenance
            {
                Id = 2,
                VehicleId = 1,
                ScheduledDate = new DateTime(2024, 2, 20),
                CompletedDate = new DateTime(2024, 2, 21),
                Description = "Brake pad replacement and tire rotation",
                Cost = 320.00m,
                Type = MaintenanceType.Repair,
                Status = MaintenanceStatus.Completed
            },
            new Maintenance
            {
                Id = 3,
                VehicleId = 1,
                ScheduledDate = new DateTime(2024, 3, 15),
                CompletedDate = new DateTime(2024, 3, 15),
                Description = "Annual vehicle inspection",
                Cost = 50.00m,
                Type = MaintenanceType.Inspection,
                Status = MaintenanceStatus.Completed
            },
            new Maintenance
            {
                Id = 4,
                VehicleId = 1,
                ScheduledDate = new DateTime(2024, 5, 1),
                CompletedDate = null,
                Description = "Scheduled air conditioning service",
                Cost = 150.00m,
                Type = MaintenanceType.Routine,
                Status = MaintenanceStatus.Scheduled
            }
        );

        // Seed damage records for Toyota Corolla (without rental IDs for now)
        modelBuilder.Entity<VehicleDamage>().HasData(
            new VehicleDamage
            {
                Id = 1,
                VehicleId = 1,
                RentalId = null,
                ReportedDate = new DateTime(2024, 2, 15),
                Description = "Small scratch on rear bumper, likely from parking",
                Severity = DamageSeverity.Minor,
                RepairCost = 150.00m,
                RepairedDate = new DateTime(2024, 2, 18),
                ReportedBy = "Customer",
                Status = DamageStatus.Repaired
            },
            new VehicleDamage
            {
                Id = 2,
                VehicleId = 1,
                RentalId = null,
                ReportedDate = new DateTime(2024, 3, 12),
                Description = "Dent on driver's side door, moderate damage",
                Severity = DamageSeverity.Moderate,
                RepairCost = 450.00m,
                RepairedDate = new DateTime(2024, 3, 20),
                ReportedBy = "Admin Staff",
                Status = DamageStatus.Repaired
            },
            new VehicleDamage
            {
                Id = 3,
                VehicleId = 1,
                RentalId = null,
                ReportedDate = new DateTime(2024, 4, 10),
                Description = "Windshield chip from road debris",
                Severity = DamageSeverity.Minor,
                RepairCost = 200.00m,
                RepairedDate = null,
                ReportedBy = "Employee",
                Status = DamageStatus.UnderRepair
            }
        );
    }
}
