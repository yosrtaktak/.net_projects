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
    public string UserId { get; set; } = string.Empty; // Changed from int CustomerId
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public decimal TotalCost { get; set; }
    public RentalStatus Status { get; set; }
    public int? StartMileage { get; set; }
    public int? EndMileage { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Customer? Customer { get; set; } // Keep for backward compatibility, but mapped to User
    public Vehicle? Vehicle { get; set; }
    
    // Alias properties for compatibility during migration
    [Obsolete("Use UserId instead")]
    public int CustomerId 
    { 
        get => 0; // Return dummy value for backward compatibility
        set { } // Ignore set attempts
    }
}

public class CreateRentalRequest
{
    public string UserId { get; set; } = string.Empty; // Changed from int CustomerId
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PricingStrategy { get; set; } = "standard";
    
    // Alias property for backward compatibility during migration
    [Obsolete("Use UserId instead")]
    public int CustomerId 
    { 
        get => 0;
        set { } // Ignore attempts to set CustomerId
    }
}

public class CalculatePriceRequest
{
    public int VehicleId { get; set; }
    public string UserId { get; set; } = string.Empty; // Changed from int CustomerId
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PricingStrategy { get; set; } = "standard";
    
    // Alias property for backward compatibility during migration
    [Obsolete("Use UserId instead")]
    public int CustomerId 
    { 
        get => 0;
        set { } // Ignore attempts to set CustomerId
    }
}

public class PriceCalculationResponse
{
    public decimal TotalPrice { get; set; }
    public int NumberOfDays { get; set; }
    public decimal DailyRate { get; set; }
    public string StrategyUsed { get; set; } = string.Empty;
    public decimal? Discount { get; set; }
}

public class CompleteRentalDto
{
    public int EndMileage { get; set; }
}
