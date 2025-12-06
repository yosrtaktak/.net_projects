namespace Frontend.Models;

public enum DamageSeverity
{
    Minor,      // Scratches, small dents
    Moderate,   // Larger dents, broken lights
    Major,      // Body damage, mechanical issues
    Critical    // Severe damage requiring extensive repair
}

public enum DamageStatus
{
    Reported,
    UnderRepair,
    Repaired,
    Unresolved
}

// Vehicle Damage DTOs
public class VehicleDamage
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public int? RentalId { get; set; }
    public Rental? Rental { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public DamageSeverity Severity { get; set; }
    public decimal RepairCost { get; set; }
    public DateTime? RepairedDate { get; set; }
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
    public DamageStatus Status { get; set; }
}

public class VehicleDamageDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public int? RentalId { get; set; }
    public Rental? Rental { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public DamageSeverity Severity { get; set; }
    public decimal RepairCost { get; set; }
    public DateTime? RepairedDate { get; set; }
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
    public DamageStatus Status { get; set; }
}

public class CreateVehicleDamageDto
{
    public int VehicleId { get; set; }
    public int? RentalId { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Severity { get; set; }
    public decimal? RepairCost { get; set; }
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
}

public class CreateVehicleDamageRequest
{
    public int VehicleId { get; set; }
    public int? RentalId { get; set; }
    public DateTime ReportedDate { get; set; } = DateTime.Now;
    public string Description { get; set; } = string.Empty;
    public int Severity { get; set; }
    public decimal? RepairCost { get; set; } // Make nullable so customers don't need to provide
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
}

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

public class RepairDamageDto
{
    public DateTime RepairedDate { get; set; }
    public decimal? ActualRepairCost { get; set; }
    public string? RepairNotes { get; set; }
}

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
