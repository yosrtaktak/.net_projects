using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VehiclesController(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetAllVehicles()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        return Ok(vehicles);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetVehicle(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        
        if (vehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        return Ok(vehicle);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}/history")]
    public async Task<ActionResult<object>> GetVehicleHistory(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdWithHistoryAsync(id);
        
        if (vehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        // Get all rentals for this vehicle
        var rentals = vehicle.Rentals
            .OrderByDescending(r => r.StartDate)
            .Select(r => new
            {
                r.Id,
                r.StartDate,
                r.EndDate,
                r.ActualReturnDate,
                r.TotalCost,
                r.Status,
                r.StartMileage,
                r.EndMileage,
                r.Notes,
                User = new
                {
                    r.User.Id,
                    r.User.FirstName,
                    r.User.LastName,
                    r.User.Email
                }
            })
            .ToList();

        // Get all maintenance records
        var maintenanceRecords = vehicle.MaintenanceRecords
            .OrderByDescending(m => m.ScheduledDate)
            .Select(m => new
            {
                m.Id,
                m.ScheduledDate,
                m.CompletedDate,
                m.Description,
                m.Cost,
                m.Type,
                m.Status
            })
            .ToList();

        // Get all damage records
        var damageRecords = vehicle.DamageRecords
            .OrderByDescending(d => d.ReportedDate)
            .Select(d => new
            {
                d.Id,
                d.ReportedDate,
                d.Description,
                d.Severity,
                d.RepairCost,
                d.RepairedDate,
                d.ReportedBy,
                d.ImageUrl,
                d.Status,
                d.RentalId
            })
            .ToList();

        // Calculate mileage evolution
        var mileageDataPoints = new List<object>();
        var completedRentals = vehicle.Rentals
            .Where(r => r.Status == RentalStatus.Completed && r.StartMileage.HasValue && r.EndMileage.HasValue)
            .OrderBy(r => r.StartDate)
            .ToList();

        foreach (var rental in completedRentals)
        {
            mileageDataPoints.Add(new
            {
                Date = rental.StartDate,
                Mileage = rental.StartMileage!.Value,
                EventType = "Rental Start",
                RelatedId = rental.Id
            });
            
            mileageDataPoints.Add(new
            {
                Date = rental.ActualReturnDate ?? rental.EndDate,
                Mileage = rental.EndMileage!.Value,
                EventType = "Rental End",
                RelatedId = rental.Id
            });
        }

        // Add maintenance points
        foreach (var maintenance in vehicle.MaintenanceRecords.Where(m => m.Status == MaintenanceStatus.Completed))
        {
            mileageDataPoints.Add(new
            {
                Date = maintenance.CompletedDate ?? maintenance.ScheduledDate,
                Mileage = vehicle.Mileage,
                EventType = $"Maintenance - {maintenance.Type}",
                RelatedId = maintenance.Id
            });
        }

        var totalMileageDriven = completedRentals.Any() 
            ? completedRentals.Sum(r => (r.EndMileage ?? 0) - (r.StartMileage ?? 0))
            : 0;

        var averageMileagePerRental = completedRentals.Any()
            ? (decimal)totalMileageDriven / completedRentals.Count
            : 0;

        var mileageEvolution = new
        {
            CurrentMileage = vehicle.Mileage,
            InitialMileage = completedRentals.Any() ? completedRentals.First().StartMileage ?? 0 : vehicle.Mileage,
            DataPoints = mileageDataPoints.OrderBy(dp => ((dynamic)dp).Date),
            AverageMileagePerRental = averageMileagePerRental,
            TotalMileageDriven = totalMileageDriven
        };

        var response = new
        {
            Vehicle = new
            {
                vehicle.Id,
                vehicle.Brand,
                vehicle.Model,
                vehicle.RegistrationNumber,
                vehicle.Year,
                vehicle.CategoryId,
                CategoryName = vehicle.Category?.Name,
                vehicle.DailyRate,
                vehicle.Status,
                vehicle.ImageUrl,
                vehicle.Mileage,
                vehicle.FuelType,
                vehicle.SeatingCapacity
            },
            Rentals = rentals,
            MaintenanceRecords = maintenanceRecords,
            DamageRecords = damageRecords,
            MileageEvolution = mileageEvolution
        };

        return Ok(response);
    }

    // New endpoint: Get all rentals for a specific vehicle
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}/rentals")]
    public async Task<ActionResult<object>> GetVehicleRentals(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdWithHistoryAsync(id);
        
        if (vehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        var rentals = vehicle.Rentals
            .OrderByDescending(r => r.StartDate)
            .Select(r => new
            {
                r.Id,
                r.StartDate,
                r.EndDate,
                r.ActualReturnDate,
                r.TotalCost,
                r.Status,
                r.StartMileage,
                r.EndMileage,
                r.Notes,
                r.CreatedAt,
                User = new
                {
                    r.User.Id,
                    r.User.FirstName,
                    r.User.LastName,
                    r.User.Email,
                    r.User.PhoneNumber
                },
                DistanceDriven = (r.EndMileage.HasValue && r.StartMileage.HasValue) 
                    ? r.EndMileage.Value - r.StartMileage.Value 
                    : (int?)null,
                DaysRented = (r.ActualReturnDate ?? r.EndDate).Subtract(r.StartDate).Days
            })
            .ToList();

        return Ok(new
        {
            VehicleId = id,
            TotalRentals = rentals.Count,
            CompletedRentals = rentals.Count(r => r.Status == RentalStatus.Completed),
            TotalRevenue = rentals.Where(r => r.Status == RentalStatus.Completed).Sum(r => r.TotalCost),
            TotalDistanceDriven = rentals.Where(r => r.DistanceDriven.HasValue).Sum(r => r.DistanceDriven ?? 0),
            Rentals = rentals
        });
    }

    // New endpoint: Get all maintenance records for a specific vehicle
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}/maintenances")]
    public async Task<ActionResult<object>> GetVehicleMaintenances(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdWithHistoryAsync(id);
        
        if (vehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        var maintenances = vehicle.MaintenanceRecords
            .OrderByDescending(m => m.ScheduledDate)
            .Select(m => new
            {
                m.Id,
                m.ScheduledDate,
                m.CompletedDate,
                m.Description,
                m.Cost,
                m.Type,
                m.Status,
                DaysToComplete = m.CompletedDate.HasValue 
                    ? (m.CompletedDate.Value - m.ScheduledDate).Days 
                    : (int?)null,
                IsOverdue = m.Status != MaintenanceStatus.Completed && m.ScheduledDate < DateTime.UtcNow,
                TypeName = m.Type.ToString(),
                StatusName = m.Status.ToString()
            })
            .ToList();

        return Ok(new
        {
            VehicleId = id,
            TotalMaintenances = maintenances.Count,
            CompletedMaintenances = maintenances.Count(m => m.Status == MaintenanceStatus.Completed),
            ScheduledMaintenances = maintenances.Count(m => m.Status == MaintenanceStatus.Scheduled),
            InProgressMaintenances = maintenances.Count(m => m.Status == MaintenanceStatus.InProgress),
            OverdueMaintenances = maintenances.Count(m => m.IsOverdue),
            TotalMaintenanceCost = maintenances.Where(m => m.Status == MaintenanceStatus.Completed).Sum(m => m.Cost),
            Maintenances = maintenances
        });
    }

    // New endpoint: Get all damage reports for a specific vehicle
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}/damages")]
    public async Task<ActionResult<object>> GetVehicleDamages(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdWithHistoryAsync(id);
        
        if (vehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        var damages = vehicle.DamageRecords
            .OrderByDescending(d => d.ReportedDate)
            .Select(d => new
            {
                d.Id,
                d.ReportedDate,
                d.Description,
                d.Severity,
                d.RepairCost,
                d.RepairedDate,
                d.ReportedBy,
                d.ImageUrl,
                d.Status,
                d.RentalId,
                SeverityName = d.Severity.ToString(),
                StatusName = d.Status.ToString(),
                DaysToRepair = d.RepairedDate.HasValue 
                    ? (d.RepairedDate.Value - d.ReportedDate).Days 
                    : (int?)null,
                IsUnderRepair = d.Status == DamageStatus.UnderRepair,
                RelatedRental = d.RentalId.HasValue 
                    ? vehicle.Rentals.FirstOrDefault(r => r.Id == d.RentalId) 
                    : null
            })
            .ToList();

        return Ok(new
        {
            VehicleId = id,
            TotalDamages = damages.Count,
            RepairedDamages = damages.Count(d => d.Status == DamageStatus.Repaired),
            UnderRepairDamages = damages.Count(d => d.Status == DamageStatus.UnderRepair),
            UnresolvedDamages = damages.Count(d => d.Status == DamageStatus.Unresolved),
            TotalRepairCost = damages.Where(d => d.Status == DamageStatus.Repaired).Sum(d => d.RepairCost),
            MinorDamages = damages.Count(d => d.Severity == DamageSeverity.Minor),
            ModerateDamages = damages.Count(d => d.Severity == DamageSeverity.Moderate),
            MajorDamages = damages.Count(d => d.Severity == DamageSeverity.Major),
            CriticalDamages = damages.Count(d => d.Severity == DamageSeverity.Critical),
            Damages = damages
        });
    }

    [AllowAnonymous]
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetAvailableVehicles(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        var vehicles = await _vehicleRepository.GetAvailableVehiclesAsync(startDate, endDate);
        return Ok(vehicles);
    }

    [AllowAnonymous]
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehiclesByCategory(int categoryId)
    {
        var vehicles = await _vehicleRepository.GetVehiclesByCategoryAsync(categoryId);
        return Ok(vehicles);
    }

    [AllowAnonymous]
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehiclesByStatus(VehicleStatus status)
    {
        var vehicles = await _vehicleRepository.GetVehiclesByStatusAsync(status);
        return Ok(vehicles);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPost]
    public async Task<ActionResult<Vehicle>> CreateVehicle([FromBody] Vehicle vehicle)
    {
        await _vehicleRepository.AddAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}")]
    public async Task<ActionResult<Vehicle>> UpdateVehicle(int id, [FromBody] Vehicle vehicle)
    {
        var existingVehicle = await _vehicleRepository.GetByIdAsync(id);
        
        if (existingVehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        existingVehicle.Brand = vehicle.Brand;
        existingVehicle.Model = vehicle.Model;
        existingVehicle.RegistrationNumber = vehicle.RegistrationNumber;
        existingVehicle.Year = vehicle.Year;
        existingVehicle.CategoryId = vehicle.CategoryId;
        existingVehicle.DailyRate = vehicle.DailyRate;
        existingVehicle.Status = vehicle.Status;
        existingVehicle.ImageUrl = vehicle.ImageUrl;
        existingVehicle.Mileage = vehicle.Mileage;
        existingVehicle.FuelType = vehicle.FuelType;
        existingVehicle.SeatingCapacity = vehicle.SeatingCapacity;

        _vehicleRepository.Update(existingVehicle);
        await _unitOfWork.CommitAsync();

        return Ok(existingVehicle);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVehicle(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        
        if (vehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        _vehicleRepository.Remove(vehicle);
        await _unitOfWork.CommitAsync();

        return NoContent();
    }
}
