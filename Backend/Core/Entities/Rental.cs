namespace Backend.Core.Entities;

public class Rental
{
    public int Id { get; set; }
    
    // Changed from CustomerId (int) to UserId (string)
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public DateTime? ActualReturnDate { get; set; }
    
    public decimal TotalCost { get; set; }
    
    public RentalStatus Status { get; set; }
    
    public int? StartMileage { get; set; }
    
    public int? EndMileage { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Payment? Payment { get; set; }
}

public enum RentalStatus
{
    Reserved,
    Active,
    Completed,
    Cancelled
}
