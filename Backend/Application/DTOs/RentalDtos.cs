using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs;

public class CreateRentalDto
{
    [Required]
    public int CustomerId { get; set; }

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
    [Required]
    public int VehicleId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string PricingStrategy { get; set; } = "standard";
}
