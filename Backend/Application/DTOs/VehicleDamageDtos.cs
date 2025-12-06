namespace Backend.Application.DTOs;

// Create Vehicle Damage DTO
public class CreateVehicleDamageDto
{
    public int VehicleId { get; set; }
    public int? RentalId { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Severity { get; set; } // DamageSeverity enum as int
    public decimal? RepairCost { get; set; } // Make nullable so customers don't need to provide
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
}

// Update Vehicle Damage DTO
public class UpdateVehicleDamageDto
{
    public string? Description { get; set; }
    public int? Severity { get; set; }
    public decimal? RepairCost { get; set; }
    public DateTime? RepairedDate { get; set; }
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
    public int? Status { get; set; }
}

// Repair Damage DTO
public class RepairDamageDto
{
    public DateTime RepairedDate { get; set; }
    public decimal? ActualRepairCost { get; set; }
    public string? RepairNotes { get; set; }
}

// Damage Filter DTO
public class DamageFilterDto
{
    public int? VehicleId { get; set; }
    public int? RentalId { get; set; }
    public int? Severity { get; set; }
    public int? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? UnresolvedOnly { get; set; }
}
