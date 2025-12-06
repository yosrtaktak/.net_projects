using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.Application.DTOs;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VehicleDamagesController : ControllerBase
{
    private readonly IVehicleDamageRepository _damageRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VehicleDamagesController(
        IVehicleDamageRepository damageRepository,
        IVehicleRepository vehicleRepository,
        IRentalRepository rentalRepository,
        IUnitOfWork unitOfWork)
    {
        _damageRepository = damageRepository;
        _vehicleRepository = vehicleRepository;
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
    }

    // GET: api/vehicledamages
    // Admin and Employee can view all damages
    [Authorize(Roles = "Admin,Employee")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleDamage>>> GetAllDamages(
        [FromQuery] DamageFilterDto? filter = null)
    {
        // Get damages with includes for Vehicle and Rental
        var damages = await _damageRepository.GetAllWithDetailsAsync();

        if (filter != null)
        {
            if (filter.VehicleId.HasValue)
                damages = damages.Where(d => d.VehicleId == filter.VehicleId.Value);

            if (filter.RentalId.HasValue)
                damages = damages.Where(d => d.RentalId == filter.RentalId.Value);

            if (filter.Severity.HasValue)
                damages = damages.Where(d => (int)d.Severity == filter.Severity.Value);

            if (filter.Status.HasValue)
                damages = damages.Where(d => (int)d.Status == filter.Status.Value);

            if (filter.StartDate.HasValue)
                damages = damages.Where(d => d.ReportedDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                damages = damages.Where(d => d.ReportedDate <= filter.EndDate.Value);

            if (filter.UnresolvedOnly == true)
                damages = damages.Where(d => d.Status != DamageStatus.Repaired);
        }

        return Ok(damages.OrderByDescending(d => d.ReportedDate));
    }

    // GET: api/vehicledamages/{id}
    // Admin, Employee can view any damage; Customer can view damages from their rentals
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDamage>> GetDamage(int id)
    {
        var damage = await _damageRepository.GetByIdWithDetailsAsync(id);
        
        if (damage == null)
        {
            return NotFound(new { message = "Damage record not found" });
        }

        // If customer, check if they're associated with the rental
        if (User.IsInRole("Customer"))
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
            
            if (damage.Rental?.User?.Email != User.Identity?.Name)
            {
                return Forbid();
            }
        }

        return Ok(damage);
    }

    // GET: api/vehicledamages/vehicle/{vehicleId}
    // Admin and Employee can view damage history for any vehicle
    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("vehicle/{vehicleId}")]
    public async Task<ActionResult<IEnumerable<VehicleDamage>>> GetDamagesByVehicle(int vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        var damages = await _damageRepository.GetDamagesByVehicleAsync(vehicleId);
        return Ok(damages);
    }

    // GET: api/vehicledamages/rental/{rentalId}
    // Admin, Employee, and the customer who made the rental can view
    [HttpGet("rental/{rentalId}")]
    public async Task<ActionResult<IEnumerable<VehicleDamage>>> GetDamagesByRental(int rentalId)
    {
        var rental = await _rentalRepository.GetByIdAsync(rentalId);
        if (rental == null)
        {
            return NotFound(new { message = "Rental not found" });
        }

        // If customer, verify they own the rental
        if (User.IsInRole("Customer"))
        {
            var userEmail = User.Identity?.Name;
            var rentalWithUser = await _rentalRepository.GetByIdWithDetailsAsync(rentalId);
            
            if (rentalWithUser?.User?.Email != userEmail)
            {
                return Forbid();
            }
        }

        var damages = await _damageRepository.GetDamagesByRentalAsync(rentalId);
        return Ok(damages);
    }

    // GET: api/vehicledamages/unresolved
    // Admin and Employee can view all unresolved damages
    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("unresolved")]
    public async Task<ActionResult<IEnumerable<VehicleDamage>>> GetUnresolvedDamages()
    {
        var damages = await _damageRepository.GetUnresolvedDamagesAsync();
        return Ok(damages);
    }

    // POST: api/vehicledamages
    // Admin, Employee, and Customer (for their rentals) can report damage
    [HttpPost]
    public async Task<ActionResult<VehicleDamage>> CreateDamage([FromBody] CreateVehicleDamageDto dto)
    {
        // Validate vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId);
        if (vehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        // If rental is specified, validate it exists
        if (dto.RentalId.HasValue)
        {
            var rental = await _rentalRepository.GetByIdWithDetailsAsync(dto.RentalId.Value);
            if (rental == null)
            {
                return NotFound(new { message = "Rental not found" });
            }

            // If customer, verify they own the rental
            if (User.IsInRole("Customer"))
            {
                var userEmail = User.Identity?.Name;
                if (rental.User?.Email != userEmail)
                {
                    return Forbid();
                }
            }
        }
        else if (User.IsInRole("Customer"))
        {
            // Customers must associate damage with a rental
            return BadRequest(new { message = "Customers must report damage for a specific rental" });
        }

        // Set default repair cost if not provided, based on severity
        var repairCost = dto.RepairCost ?? dto.Severity switch
        {
            0 => 100m,  // Minor
            1 => 300m,  // Moderate
            2 => 800m,  // Major
            3 => 2000m, // Critical
            _ => 100m
        };

        var damage = new VehicleDamage
        {
            VehicleId = dto.VehicleId,
            RentalId = dto.RentalId,
            ReportedDate = dto.ReportedDate,
            Description = dto.Description,
            Severity = (DamageSeverity)dto.Severity,
            RepairCost = repairCost,
            ReportedBy = dto.ReportedBy ?? User.Identity?.Name ?? "Unknown",
            ImageUrl = dto.ImageUrl,
            Status = DamageStatus.Reported
        };

        await _damageRepository.AddAsync(damage);
        await _unitOfWork.CommitAsync();

        // If severe damage, set vehicle to Maintenance status
        if (damage.Severity >= DamageSeverity.Major && vehicle.Status == VehicleStatus.Available)
        {
            vehicle.Status = VehicleStatus.Maintenance;
            _vehicleRepository.Update(vehicle);
            await _unitOfWork.CommitAsync();
        }

        return CreatedAtAction(nameof(GetDamage), new { id = damage.Id }, damage);
    }

    // PUT: api/vehicledamages/{id}
    // Only Admin and Employee can update damage records
    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleDamage>> UpdateDamage(int id, [FromBody] UpdateVehicleDamageDto dto)
    {
        var damage = await _damageRepository.GetByIdWithDetailsAsync(id);
        
        if (damage == null)
        {
            return NotFound(new { message = "Damage record not found" });
        }

        // Update only provided fields
        if (dto.Description != null)
            damage.Description = dto.Description;
        
        if (dto.Severity.HasValue)
            damage.Severity = (DamageSeverity)dto.Severity.Value;
        
        if (dto.RepairCost.HasValue)
            damage.RepairCost = dto.RepairCost.Value;
        
        if (dto.RepairedDate.HasValue)
            damage.RepairedDate = dto.RepairedDate.Value;
        
        if (dto.ReportedBy != null)
            damage.ReportedBy = dto.ReportedBy;
        
        if (dto.ImageUrl != null)
            damage.ImageUrl = dto.ImageUrl;
        
        if (dto.Status.HasValue)
            damage.Status = (DamageStatus)dto.Status.Value;

        _damageRepository.Update(damage);
        await _unitOfWork.CommitAsync();

        return Ok(damage);
    }

    // PUT: api/vehicledamages/{id}/repair
    // Only Admin and Employee can mark damage as repaired
    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}/repair")]
    public async Task<ActionResult<VehicleDamage>> RepairDamage(int id, [FromBody] RepairDamageDto dto)
    {
        var damage = await _damageRepository.GetByIdWithDetailsAsync(id);
        
        if (damage == null)
        {
            return NotFound(new { message = "Damage record not found" });
        }

        if (damage.Status == DamageStatus.Repaired)
        {
            return BadRequest(new { message = "Damage is already repaired" });
        }

        damage.RepairedDate = dto.RepairedDate;
        damage.Status = DamageStatus.Repaired;
        
        if (dto.ActualRepairCost.HasValue)
            damage.RepairCost = dto.ActualRepairCost.Value;

        _damageRepository.Update(damage);

        // Check if vehicle can be set back to Available
        if (damage.Vehicle.Status == VehicleStatus.Maintenance)
        {
            var unresolvedDamages = await _damageRepository.GetDamagesByVehicleAsync(damage.VehicleId);
            var hasUnresolvedDamages = unresolvedDamages.Any(d => 
                d.Id != id && d.Status != DamageStatus.Repaired);

            // Also check for pending maintenances
            var vehicle = await _vehicleRepository.GetByIdWithHistoryAsync(damage.VehicleId);
            var hasPendingMaintenance = vehicle?.MaintenanceRecords.Any(m => 
                m.Status == MaintenanceStatus.Scheduled || m.Status == MaintenanceStatus.InProgress) ?? false;

            if (!hasUnresolvedDamages && !hasPendingMaintenance && vehicle != null)
            {
                vehicle.Status = VehicleStatus.Available;
                _vehicleRepository.Update(vehicle);
            }
        }

        await _unitOfWork.CommitAsync();

        return Ok(damage);
    }

    // PUT: api/vehicledamages/{id}/start-repair
    // Only Admin and Employee can start repair
    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}/start-repair")]
    public async Task<ActionResult<VehicleDamage>> StartRepair(int id)
    {
        var damage = await _damageRepository.GetByIdWithDetailsAsync(id);
        
        if (damage == null)
        {
            return NotFound(new { message = "Damage record not found" });
        }

        if (damage.Status != DamageStatus.Reported)
        {
            return BadRequest(new { message = "Can only start repair on reported damages" });
        }

        damage.Status = DamageStatus.UnderRepair;
        _damageRepository.Update(damage);

        // Set vehicle to Maintenance status
        if (damage.Vehicle.Status == VehicleStatus.Available)
        {
            damage.Vehicle.Status = VehicleStatus.Maintenance;
            _vehicleRepository.Update(damage.Vehicle);
        }

        await _unitOfWork.CommitAsync();

        return Ok(damage);
    }

    // DELETE: api/vehicledamages/{id}
    // Only Admin can delete damage records
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDamage(int id)
    {
        var damage = await _damageRepository.GetByIdAsync(id);
        
        if (damage == null)
        {
            return NotFound(new { message = "Damage record not found" });
        }

        _damageRepository.Remove(damage);
        await _unitOfWork.CommitAsync();

        return NoContent();
    }
}
