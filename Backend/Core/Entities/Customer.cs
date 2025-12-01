using System.ComponentModel.DataAnnotations;

namespace Backend.Core.Entities;

public class Customer
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public string DriverLicenseNumber { get; set; } = string.Empty;
    
    public DateTime DateOfBirth { get; set; }
    
    public string? Address { get; set; }
    
    public DateTime RegistrationDate { get; set; }
    
    public CustomerTier Tier { get; set; } = CustomerTier.Standard;
    
    // Navigation properties
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}

public enum CustomerTier
{
    Standard,
    Silver,
    Gold,
    Platinum
}
