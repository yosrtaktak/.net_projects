using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.Application.DTOs;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MaintenancesController : ControllerBase
{
    private readonly IMaintenanceRepository _maintenanceRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MaintenancesController(
        IMaintenanceRepository maintenanceRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _maintenanceRepository = maintenanceRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    // GET: api/maintenances
    // Admin and Employee can view all maintenances
    [Authorize(Roles = "Admin,Employee")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Maintenance>>> GetAllMaintenances(
        [FromQuery] MaintenanceFilterDto? filter = null)
    {
        // Get maintenances with includes for Vehicle
        var maintenances = await _maintenanceRepository.GetAllWithDetailsAsync();

        if (filter != null)
        {
            if (filter.VehicleId.HasValue)
                maintenances = maintenances.Where(m => m.VehicleId == filter.VehicleId.Value);

            if (filter.Type.HasValue)
                maintenances = maintenances.Where(m => (int)m.Type == filter.Type.Value);

            if (filter.Status.HasValue)
                maintenances = maintenances.Where(m => (int)m.Status == filter.Status.Value);

            if (filter.StartDate.HasValue)
                maintenances = maintenances.Where(m => m.ScheduledDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                maintenances = maintenances.Where(m => m.ScheduledDate <= filter.EndDate.Value);

            if (filter.IsOverdue == true)
            {
                var today = DateTime.UtcNow.Date;
                maintenances = maintenances.Where(m => 
                    m.Status == MaintenanceStatus.Scheduled && m.ScheduledDate < today);
            }
        }

        return Ok(maintenances.OrderByDescending(m => m.ScheduledDate));
    }

    // GET: api/maintenances/{id}
    // Admin, Employee, and Customer (for their rentals) can view specific maintenance
    [HttpGet("{id}")]
    public async Task<ActionResult<Maintenance>> GetMaintenance(int id)
    {
        var maintenance = await _maintenanceRepository.GetByIdWithDetailsAsync(id);
        
        if (maintenance == null)
        {
            return NotFound(new { message = "Maintenance record not found" });
        }

        // Customer users cannot view maintenances (they're vehicle-specific, not customer-specific)
        if (User.IsInRole("Customer"))
        {
            return Forbid();
        }

        return Ok(maintenance);
    }

    // GET: api/maintenances/vehicle/{vehicleId}
    // Admin and Employee can view maintenance history for any vehicle
    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("vehicle/{vehicleId}")]
    public async Task<ActionResult<IEnumerable<Maintenance>>> GetMaintenancesByVehicle(int vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        var maintenances = await _maintenanceRepository.GetMaintenancesByVehicleAsync(vehicleId);
        return Ok(maintenances);
    }

    // GET: api/maintenances/overdue
    // Admin and Employee can view overdue maintenances
    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<Maintenance>>> GetOverdueMaintenances()
    {
        var maintenances = await _maintenanceRepository.GetOverdueMaintenancesAsync();
        return Ok(maintenances);
    }

    // GET: api/maintenances/scheduled
    // Admin and Employee can view scheduled maintenances
    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("scheduled")]
    public async Task<ActionResult<IEnumerable<Maintenance>>> GetScheduledMaintenances(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.Date;
        var end = endDate ?? DateTime.UtcNow.Date.AddDays(30);

        var maintenances = await _maintenanceRepository.GetScheduledMaintenancesAsync(start, end);
        return Ok(maintenances);
    }

    // POST: api/maintenances
    // Only Admin and Employee can schedule maintenance
    [Authorize(Roles = "Admin,Employee")]
    [HttpPost]
    public async Task<ActionResult<Maintenance>> CreateMaintenance([FromBody] CreateMaintenanceDto dto)
    {
        // Validate vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId);
        if (vehicle == null)
        {
            return NotFound(new { message = "Vehicle not found" });
        }

        var maintenance = new Maintenance
        {
            VehicleId = dto.VehicleId,
            ScheduledDate = dto.ScheduledDate,
            Description = dto.Description,
            Cost = dto.Cost,
            Type = (MaintenanceType)dto.Type,
            Status = MaintenanceStatus.Scheduled
        };

        await _maintenanceRepository.AddAsync(maintenance);
        await _unitOfWork.CommitAsync();

        // If vehicle is currently Available, set it to Maintenance status if scheduled soon
        if (vehicle.Status == VehicleStatus.Available && 
            maintenance.ScheduledDate <= DateTime.UtcNow.Date.AddDays(1))
        {
            vehicle.Status = VehicleStatus.Maintenance;
            _vehicleRepository.Update(vehicle);
            await _unitOfWork.CommitAsync();
        }

        return CreatedAtAction(nameof(GetMaintenance), new { id = maintenance.Id }, maintenance);
    }

    // PUT: api/maintenances/{id}
    // Only Admin and Employee can update maintenance
    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}")]
    public async Task<ActionResult<Maintenance>> UpdateMaintenance(int id, [FromBody] UpdateMaintenanceDto dto)
    {
        var maintenance = await _maintenanceRepository.GetByIdWithDetailsAsync(id);
        
        if (maintenance == null)
        {
            return NotFound(new { message = "Maintenance record not found" });
        }

        // Update only provided fields
        if (dto.ScheduledDate.HasValue)
            maintenance.ScheduledDate = dto.ScheduledDate.Value;
        
        if (dto.CompletedDate.HasValue)
            maintenance.CompletedDate = dto.CompletedDate.Value;
        
        if (dto.Description != null)
            maintenance.Description = dto.Description;
        
        if (dto.Cost.HasValue)
            maintenance.Cost = dto.Cost.Value;
        
        if (dto.Type.HasValue)
            maintenance.Type = (MaintenanceType)dto.Type.Value;
        
        if (dto.Status.HasValue)
            maintenance.Status = (MaintenanceStatus)dto.Status.Value;

        _maintenanceRepository.Update(maintenance);
        await _unitOfWork.CommitAsync();

        return Ok(maintenance);
    }

    // PUT: api/maintenances/{id}/complete
    // Only Admin and Employee can complete maintenance
    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}/complete")]
    public async Task<ActionResult<Maintenance>> CompleteMaintenance(int id, [FromBody] CompleteMaintenanceDto dto)
    {
        var maintenance = await _maintenanceRepository.GetByIdWithDetailsAsync(id);
        
        if (maintenance == null)
        {
            return NotFound(new { message = "Maintenance record not found" });
        }

        if (maintenance.Status == MaintenanceStatus.Completed)
        {
            return BadRequest(new { message = "Maintenance is already completed" });
        }

        maintenance.CompletedDate = dto.CompletedDate;
        maintenance.Status = MaintenanceStatus.Completed;
        
        if (dto.ActualCost.HasValue)
            maintenance.Cost = dto.ActualCost.Value;

        _maintenanceRepository.Update(maintenance);

        // Update vehicle status back to Available if it was in Maintenance
        if (maintenance.Vehicle.Status == VehicleStatus.Maintenance)
        {
            // Check if there are other pending maintenances
            var pendingMaintenances = await _maintenanceRepository.GetMaintenancesByVehicleAsync(maintenance.VehicleId);
            var hasPendingMaintenance = pendingMaintenances.Any(m => 
                m.Id != id && 
                (m.Status == MaintenanceStatus.Scheduled || m.Status == MaintenanceStatus.InProgress));

            if (!hasPendingMaintenance)
            {
                maintenance.Vehicle.Status = VehicleStatus.Available;
                _vehicleRepository.Update(maintenance.Vehicle);
            }
        }

        await _unitOfWork.CommitAsync();

        return Ok(maintenance);
    }

    // PUT: api/maintenances/{id}/cancel
    // Only Admin can cancel maintenance
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<Maintenance>> CancelMaintenance(int id)
    {
        var maintenance = await _maintenanceRepository.GetByIdWithDetailsAsync(id);
        
        if (maintenance == null)
        {
            return NotFound(new { message = "Maintenance record not found" });
        }

        if (maintenance.Status == MaintenanceStatus.Completed)
        {
            return BadRequest(new { message = "Cannot cancel completed maintenance" });
        }

        maintenance.Status = MaintenanceStatus.Cancelled;
        _maintenanceRepository.Update(maintenance);

        // Update vehicle status if it was in Maintenance
        if (maintenance.Vehicle.Status == VehicleStatus.Maintenance)
        {
            // Check if there are other pending maintenances for this vehicle
            var pendingMaintenances = await _maintenanceRepository.GetMaintenancesByVehicleAsync(maintenance.VehicleId);
            var hasPendingMaintenance = pendingMaintenances.Any(m => 
                m.Id != id && 
                (m.Status == MaintenanceStatus.Scheduled || m.Status == MaintenanceStatus.InProgress));

            if (!hasPendingMaintenance)
            {
                // No more pending maintenance, set vehicle back to Available
                maintenance.Vehicle.Status = VehicleStatus.Available;
                _vehicleRepository.Update(maintenance.Vehicle);
            }
        }

        await _unitOfWork.CommitAsync();

        return Ok(maintenance);
    }

    // DELETE: api/maintenances/{id}
    // Only Admin can delete maintenance records
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMaintenance(int id)
    {
        var maintenance = await _maintenanceRepository.GetByIdWithDetailsAsync(id);
        
        if (maintenance == null)
        {
            return NotFound(new { message = "Maintenance record not found" });
        }

        var vehicleId = maintenance.VehicleId;
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

        _maintenanceRepository.Remove(maintenance);
        await _unitOfWork.CommitAsync();

        // Update vehicle status if it was in Maintenance
        if (vehicle != null && vehicle.Status == VehicleStatus.Maintenance)
        {
            // Check if there are other pending maintenances for this vehicle
            var remainingMaintenances = await _maintenanceRepository.GetMaintenancesByVehicleAsync(vehicleId);
            var hasPendingMaintenance = remainingMaintenances.Any(m => 
                m.Status == MaintenanceStatus.Scheduled || 
                m.Status == MaintenanceStatus.InProgress);

            if (!hasPendingMaintenance)
            {
                // No more pending maintenance, set vehicle back to Available
                vehicle.Status = VehicleStatus.Available;
                _vehicleRepository.Update(vehicle);
                await _unitOfWork.CommitAsync();
            }
        }

        return NoContent();
    }
}
