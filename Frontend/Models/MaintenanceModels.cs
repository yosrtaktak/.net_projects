namespace Frontend.Models;

// Maintenance Enums
public enum MaintenanceType
{
    Routine,
    Repair,
    Inspection,
    Emergency
}

public enum MaintenanceStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}

// Maintenance DTOs
public class MaintenanceDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public MaintenanceType Type { get; set; }
    public MaintenanceStatus Status { get; set; }
}

public class CreateMaintenanceDto
{
    public int VehicleId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public int Type { get; set; }
}

public class UpdateMaintenanceDto
{
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Description { get; set; }
    public decimal? Cost { get; set; }
    public int? Type { get; set; }
    public int? Status { get; set; }
}

public class CompleteMaintenanceDto
{
    public DateTime CompletedDate { get; set; }
    public decimal? ActualCost { get; set; }
    public string? Notes { get; set; }
}

public class MaintenanceFilterDto
{
    public int? VehicleId { get; set; }
    public int? Type { get; set; }
    public int? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsOverdue { get; set; }
}
