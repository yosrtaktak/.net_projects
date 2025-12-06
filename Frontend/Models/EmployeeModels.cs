namespace Frontend.Models;

public class EmployeeModel
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime? LastLogin { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class CreateEmployeeModel
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee"; // Employee or Admin
}

public class UpdateCustomerModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DriverLicenseNumber { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public CustomerTier Tier { get; set; }
}

public class UpdateProfileModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DriverLicenseNumber { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
}
