using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs;

public class CreateRentalDto
{
    public string UserId { get; set; } = null!; // Changed from int CustomerId

    [Required]
    public int VehicleId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string PricingStrategy { get; set; } = "standard";
}

public class CompleteRentalDto
{
    [Required]
    public int EndMileage { get; set; }
}

public class CalculatePriceDto
{
    public int VehicleId { get; set; }

    public string UserId { get; set; } = null!; // Changed from int CustomerId

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
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

public class UpdateRentalStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
