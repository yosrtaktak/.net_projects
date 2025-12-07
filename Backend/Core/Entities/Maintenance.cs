using System.Text.Json.Serialization;

namespace Backend.Core.Entities;

public class Maintenance
{
    public int Id { get; set; }
    
    public int VehicleId { get; set; }
    
    [JsonIgnore]
    public Vehicle Vehicle { get; set; } = null!;
    
    public DateTime ScheduledDate { get; set; }
    
    public DateTime? CompletedDate { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public decimal Cost { get; set; }
    
    public MaintenanceType Type { get; set; }
    
    public MaintenanceStatus Status { get; set; }
}

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
