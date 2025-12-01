namespace Backend.Core.Entities;

public class VehicleDamage
{
    public int Id { get; set; }
    
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    
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
