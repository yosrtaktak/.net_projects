namespace Frontend.Models;

public enum RentalStatus
{
    Reserved,
    Active,
    Completed,
    Cancelled
}

public class Rental
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalCost { get; set; }
    public RentalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Customer? Customer { get; set; }
    public Vehicle? Vehicle { get; set; }
}

public class CreateRentalRequest
{
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PricingStrategy { get; set; } = "standard";
}

public class CalculatePriceRequest
{
    public int VehicleId { get; set; }
    public int CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PricingStrategy { get; set; } = "standard";
}

public class PriceCalculationResponse
{
    public decimal TotalPrice { get; set; }
    public int NumberOfDays { get; set; }
    public decimal DailyRate { get; set; }
    public string StrategyUsed { get; set; } = string.Empty;
    public decimal? Discount { get; set; }
}
