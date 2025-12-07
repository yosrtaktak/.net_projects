using Backend.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs;

public class UserProfileDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string DriverLicenseNumber { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public DateTime RegistrationDate { get; set; }
    public CustomerTier Tier { get; set; }
    public string? UserName { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class UpdateProfileDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = "";
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = "";
    
    [Required]
    [StringLength(20, MinimumLength = 8)]
    public string PhoneNumber { get; set; } = "";
    
    [Required]
    [StringLength(20, MinimumLength = 5)]
    public string DriverLicenseNumber { get; set; } = "";
    
    public DateTime? DateOfBirth { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
}

public class CustomerListDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public CustomerTier Tier { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int TotalRentals { get; set; }
}

public class EmployeeListDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public DateTime RegistrationDate { get; set; }
    public DateTime? LastLogin { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class CreateEmployeeDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = "";
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = "";
    
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = "";
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = "";
    
    [Required]
    public string Role { get; set; } = "Employee"; // Employee or Admin
}

public class UpdateUserRoleDto
{
    [Required]
    public string Role { get; set; } = "";
}

public class UpdateTierDto
{
    public CustomerTier Tier { get; set; }
}

public class AdminUpdateCustomerDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = "";
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = "";
    
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = "";
    
    [StringLength(20)]
    public string DriverLicenseNumber { get; set; } = "";
    
    public DateTime? DateOfBirth { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
    
    public CustomerTier Tier { get; set; }
}
