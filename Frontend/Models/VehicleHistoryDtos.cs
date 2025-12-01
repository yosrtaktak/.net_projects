namespace Frontend.Models;

// Vehicle History DTOs
public class VehicleHistoryResponse
{
    public Vehicle Vehicle { get; set; } = null!;
    public List<RentalHistoryDto> Rentals { get; set; } = new();
    public List<MaintenanceHistoryDto> MaintenanceRecords { get; set; } = new();
    public List<DamageHistoryDto> DamageRecords { get; set; } = new();
    public MileageEvolutionDto MileageEvolution { get; set; } = null!;
}

public class RentalHistoryDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public decimal TotalCost { get; set; }
    public RentalStatus Status { get; set; }
    public int? StartMileage { get; set; }
    public int? EndMileage { get; set; }
    public string? Notes { get; set; }
    public CustomerDto? Customer { get; set; }
}

public class MaintenanceHistoryDto
{
    public int Id { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public MaintenanceType Type { get; set; }
    public MaintenanceStatus Status { get; set; }
}

public class DamageHistoryDto
{
    public int Id { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public DamageSeverity Severity { get; set; }
    public decimal RepairCost { get; set; }
    public DateTime? RepairedDate { get; set; }
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
    public DamageStatus Status { get; set; }
    public int? RentalId { get; set; }
}

public class MileageEvolutionDto
{
    public int CurrentMileage { get; set; }
    public int InitialMileage { get; set; }
    public List<MileageDataPoint> DataPoints { get; set; } = new();
    public decimal AverageMileagePerRental { get; set; }
    public int TotalMileageDriven { get; set; }
}

public class MileageDataPoint
{
    public DateTime Date { get; set; }
    public int Mileage { get; set; }
    public string EventType { get; set; } = string.Empty; // "Rental Start", "Rental End", "Maintenance"
    public int? RelatedId { get; set; }
}

public class CustomerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
