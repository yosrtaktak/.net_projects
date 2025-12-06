using Microsoft.AspNetCore.Identity;

namespace Backend.Core.Entities;

public class ApplicationUser : IdentityUser
{
    // Basic user info
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    
    // Customer-specific fields (moved from Customers table)
    public string? DriverLicenseNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public CustomerTier Tier { get; set; } = CustomerTier.Standard;
    
    // Navigation properties
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
