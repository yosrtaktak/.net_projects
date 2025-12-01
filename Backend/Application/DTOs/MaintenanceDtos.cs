namespace Backend.Application.DTOs;

// Create Maintenance DTO
public class CreateMaintenanceDto
{
    public int VehicleId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public int Type { get; set; } // MaintenanceType enum as int
}

// Update Maintenance DTO
public class UpdateMaintenanceDto
{
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Description { get; set; }
    public decimal? Cost { get; set; }
    public int? Type { get; set; }
    public int? Status { get; set; }
}

// Complete Maintenance DTO
public class CompleteMaintenanceDto
{
    public DateTime CompletedDate { get; set; }
    public decimal? ActualCost { get; set; }
    public string? Notes { get; set; }
}

// Maintenance Filter DTO
public class MaintenanceFilterDto
{
    public int? VehicleId { get; set; }
    public int? Type { get; set; }
    public int? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsOverdue { get; set; }
}
