namespace Frontend.Models;

public enum CustomerTier
{
    Standard,
    Silver,
    Gold,
    Platinum
}

// Main user profile model (replaces Customer)
public class UserProfile
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DriverLicenseNumber { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public DateTime RegistrationDate { get; set; }
    public CustomerTier Tier { get; set; }
}

// Keep Customer as alias for backward compatibility (temporary)
public class Customer : UserProfile
{
    // This allows existing code to continue working during migration
    // Remove this class once all references are updated to UserProfile
}
