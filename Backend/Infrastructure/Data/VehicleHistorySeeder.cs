using Microsoft.EntityFrameworkCore;
using Backend.Core.Entities;
using Backend.Infrastructure.Data;

namespace Backend.Infrastructure.Data;

public static class VehicleHistorySeeder
{
    public static void SeedVehicleHistory(this ModelBuilder modelBuilder)
    {
        var vehicleId = 1; // Toyota Corolla
        var customerId = 1; // John Doe

        // Seed some past rentals for the Toyota Corolla
        modelBuilder.Entity<Rental>().HasData(
            new Rental
            {
                Id = 1,
                CustomerId = customerId,
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
            },
            new Rental
            {
                Id = 2,
                CustomerId = customerId,
                VehicleId = vehicleId,
                StartDate = new DateTime(2024, 2, 10),
                EndDate = new DateTime(2024, 2, 15),
                ActualReturnDate = new DateTime(2024, 2, 15),
                TotalCost = 175.00m,
                Status = RentalStatus.Completed,
                StartMileage = 15250,
                EndMileage = 15480,
                Notes = "Weekend trip rental",
                CreatedAt = new DateTime(2024, 2, 5)
            },
            new Rental
            {
                Id = 3,
                CustomerId = customerId,
                VehicleId = vehicleId,
                StartDate = new DateTime(2024, 3, 5),
                EndDate = new DateTime(2024, 3, 12),
                ActualReturnDate = new DateTime(2024, 3, 12),
                TotalCost = 245.00m,
                Status = RentalStatus.Completed,
                StartMileage = 15480,
                EndMileage = 15820,
                Notes = "Business trip rental",
                CreatedAt = new DateTime(2024, 3, 1)
            },
            new Rental
            {
                Id = 4,
                CustomerId = customerId,
                VehicleId = vehicleId,
                StartDate = new DateTime(2024, 4, 1),
                EndDate = new DateTime(2024, 4, 5),
                ActualReturnDate = new DateTime(2024, 4, 6),
                TotalCost = 175.00m,
                Status = RentalStatus.Completed,
                StartMileage = 15820,
                EndMileage = 16000,
                Notes = "Returned 1 day late, extra charges applied",
                CreatedAt = new DateTime(2024, 3, 28)
            }
        );

        // Seed maintenance records for Toyota Corolla
        modelBuilder.Entity<Maintenance>().HasData(
            new Maintenance
            {
                Id = 1,
                VehicleId = vehicleId,
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
                VehicleId = vehicleId,
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
                VehicleId = vehicleId,
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
                VehicleId = vehicleId,
                ScheduledDate = new DateTime(2024, 5, 1),
                CompletedDate = null,
                Description = "Scheduled air conditioning service",
                Cost = 150.00m,
                Type = MaintenanceType.Routine,
                Status = MaintenanceStatus.Scheduled
            }
        );

        // Seed damage records for Toyota Corolla
        modelBuilder.Entity<VehicleDamage>().HasData(
            new VehicleDamage
            {
                Id = 1,
                VehicleId = vehicleId,
                RentalId = 2,
                ReportedDate = new DateTime(2024, 2, 15),
                Description = "Small scratch on rear bumper, likely from parking",
                Severity = DamageSeverity.Minor,
                RepairCost = 150.00m,
                RepairedDate = new DateTime(2024, 2, 18),
                ReportedBy = "John Doe",
                Status = DamageStatus.Repaired
            },
            new VehicleDamage
            {
                Id = 2,
                VehicleId = vehicleId,
                RentalId = 3,
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
                VehicleId = vehicleId,
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
