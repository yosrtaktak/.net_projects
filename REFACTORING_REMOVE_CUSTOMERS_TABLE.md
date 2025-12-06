# Refactoring Plan: Remove Customers Table - Use AspNetUsers Instead

## Overview
This refactoring removes the redundant `Customers` table and uses `AspNetUsers` (with Customer role) directly for customer data.

## Current Architecture Problems
1. **Data Duplication**: Email, name stored in both `AspNetUsers` and `Customers`
2. **Sync Issues**: Changes to user profile require updating two tables
3. **Complexity**: Need to maintain relationship between `AspNetUsers` and `Customers`
4. **404 Errors**: Users with Customer role but no Customer record cause issues

## New Architecture

### Use `AspNetUsers` with Extended Properties
Instead of:
```
AspNetUsers (authentication) ? Customers (profile) ? Rentals
```

We'll have:
```
AspNetUsers (authentication + profile) ? Rentals
```

## Step-by-Step Refactoring

### Phase 1: Extend ApplicationUser

**File**: `Backend/Core/Entities/ApplicationUser.cs`

```csharp
using Microsoft.AspNetCore.Identity;

namespace Backend.Core.Entities;

public class ApplicationUser : IdentityUser
{
    // Basic info (already exists)
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    
    // Customer-specific fields (NEW)
    public string? DriverLicenseNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
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
```

### Phase 2: Update Rental Entity

**File**: `Backend/Core/Entities/Rental.cs`

**Before:**
```csharp
public class Rental
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    // ...
}
```

**After:**
```csharp
public class Rental
{
    public string UserId { get; set; } = null!; // Changed from int to string
    public ApplicationUser User { get; set; } = null!; // Changed from Customer
    
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    
    // ... rest remains same
}
```

### Phase 3: Update DbContext

**File**: `Backend/Infrastructure/Data/CarRentalDbContext.cs`

**Remove:**
```csharp
public DbSet<Customer> Customers { get; set; } // DELETE THIS
```

**Update Rental Configuration:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ... Vehicle configuration remains same ...

    // REMOVE Customer configuration entirely
    // DELETE: modelBuilder.Entity<Customer>(entity => { ... });

    // Rental configuration (UPDATED)
    modelBuilder.Entity<Rental>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");
        
        // Changed from Customer to ApplicationUser
        entity.HasOne(e => e.User)
            .WithMany(u => u.Rentals)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(e => e.Vehicle)
            .WithMany(v => v.Rentals)
            .HasForeignKey(e => e.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    });
    
    // ... rest remains same ...
}
```

### Phase 4: Create Migration

**Create migration to:**
1. Add customer fields to `AspNetUsers` table
2. Migrate data from `Customers` to `AspNetUsers`
3. Update `Rentals.CustomerId` to `Rentals.UserId` (int ? string)
4. Drop `Customers` table

**Migration File**: `Backend/Migrations/[Timestamp]_RemoveCustomersTable.cs`

```csharp
using Microsoft.EntityFrameworkCore.Migrations;

public partial class RemoveCustomersTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Step 1: Add new columns to AspNetUsers
        migrationBuilder.AddColumn<string>(
            name: "DriverLicenseNumber",
            table: "AspNetUsers",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DateOfBirth",
            table: "AspNetUsers",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Address",
            table: "AspNetUsers",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "RegistrationDate",
            table: "AspNetUsers",
            type: "datetime2",
            nullable: false,
            defaultValueSql: "GETUTCDATE()");

        migrationBuilder.AddColumn<int>(
            name: "Tier",
            table: "AspNetUsers",
            type: "int",
            nullable: false,
            defaultValue: 0);

        // Step 2: Migrate customer data to AspNetUsers
        migrationBuilder.Sql(@"
            UPDATE u
            SET 
                u.FirstName = c.FirstName,
                u.LastName = c.LastName,
                u.PhoneNumber = c.PhoneNumber,
                u.DriverLicenseNumber = c.DriverLicenseNumber,
                u.DateOfBirth = c.DateOfBirth,
                u.Address = c.Address,
                u.RegistrationDate = c.RegistrationDate,
                u.Tier = c.Tier
            FROM AspNetUsers u
            INNER JOIN Customers c ON u.Email = c.Email
        ");

        // Step 3: Add new UserId column to Rentals (nullable temporarily)
        migrationBuilder.AddColumn<string>(
            name: "UserId",
            table: "Rentals",
            type: "nvarchar(450)",
            nullable: true);

        // Step 4: Populate UserId from CustomerId via email
        migrationBuilder.Sql(@"
            UPDATE r
            SET r.UserId = u.Id
            FROM Rentals r
            INNER JOIN Customers c ON r.CustomerId = c.Id
            INNER JOIN AspNetUsers u ON c.Email = u.Email
        ");

        // Step 5: Make UserId required
        migrationBuilder.AlterColumn<string>(
            name: "UserId",
            table: "Rentals",
            type: "nvarchar(450)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)",
            oldNullable: true);

        // Step 6: Drop foreign key and CustomerId column
        migrationBuilder.DropForeignKey(
            name: "FK_Rentals_Customers_CustomerId",
            table: "Rentals");

        migrationBuilder.DropIndex(
            name: "IX_Rentals_CustomerId",
            table: "Rentals");

        migrationBuilder.DropColumn(
            name: "CustomerId",
            table: "Rentals");

        // Step 7: Create new foreign key
        migrationBuilder.CreateIndex(
            name: "IX_Rentals_UserId",
            table: "Rentals",
            column: "UserId");

        migrationBuilder.AddForeignKey(
            name: "FK_Rentals_AspNetUsers_UserId",
            table: "Rentals",
            column: "UserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        // Step 8: Drop Customers table
        migrationBuilder.DropTable(
            name: "Customers");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Reverse migration (recreate Customers table)
        // ... (implementation for rollback)
    }
}
```

### Phase 5: Update Controllers

#### CustomersController ? UsersController

**NEW File**: `Backend/Controllers/UsersController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Backend.Core.Entities;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: api/users/me
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMyProfile()
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var profile = new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            DriverLicenseNumber = user.DriverLicenseNumber ?? "",
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            RegistrationDate = user.RegistrationDate,
            Tier = user.Tier
        };

        return Ok(profile);
    }

    // PUT: api/users/me
    [HttpPut("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateMyProfile([FromBody] UpdateProfileDto dto)
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;
        user.DriverLicenseNumber = dto.DriverLicenseNumber;
        user.DateOfBirth = dto.DateOfBirth;
        user.Address = dto.Address;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return await GetMyProfile();
    }

    // Admin-only: Get all customers
    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("customers")]
    public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetAllCustomers()
    {
        var users = _userManager.Users.ToList();
        var customers = new List<UserProfileDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Customer"))
            {
                customers.Add(new UserProfileDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    DriverLicenseNumber = user.DriverLicenseNumber ?? "",
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    RegistrationDate = user.RegistrationDate,
                    Tier = user.Tier
                });
            }
        }

        return Ok(customers);
    }
}
```

**DTOs**:
```csharp
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
}

public class UpdateProfileDto
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string DriverLicenseNumber { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
}
```

#### Update RentalsController

**File**: `Backend/Controllers/RentalsController.cs`

**Changes:**
```csharp
[HttpPost]
public async Task<ActionResult<Rental>> CreateRental([FromBody] CreateRentalDto dto)
{
    try
    {
        string userId;
        
        if (User.IsInRole("Customer"))
        {
            // Get user ID from JWT
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            userId = user.Id; // Use UserId directly
        }
        else
        {
            // Admin/Employee can specify any user
            userId = dto.UserId; // Changed from CustomerId
        }

        var rental = await _rentalService.CreateRentalAsync(
            userId,  // Changed from customerId
            dto.VehicleId, 
            dto.StartDate, 
            dto.EndDate, 
            dto.PricingStrategy);

        return CreatedAtAction(nameof(GetRental), new { id = rental.Id }, rental);
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
```

### Phase 6: Update Services

**File**: `Backend/Application/Services/RentalService.cs`

**Changes:**
```csharp
public interface IRentalService
{
    Task<Rental> CreateRentalAsync(
        string userId,  // Changed from int customerId
        int vehicleId, 
        DateTime startDate, 
        DateTime endDate, 
        string pricingStrategy = "standard");
    
    Task<IEnumerable<Rental>> GetRentalsByUserAsync(string userId); // Changed
    // ... other methods
}

public class RentalService : IRentalService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RentalService(
        IUnitOfWork unitOfWork,
        IRentalRepository rentalRepository,
        IVehicleRepository vehicleRepository,
        IPricingStrategyFactory pricingStrategyFactory,
        UserManager<ApplicationUser> userManager) // NEW
    {
        _userManager = userManager;
        // ... other assignments
    }

    public async Task<Rental> CreateRentalAsync(
        string userId, 
        int vehicleId, 
        DateTime startDate, 
        DateTime endDate, 
        string pricingStrategy = "standard")
    {
        // Check user exists
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        // Verify user has Customer role
        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains("Customer"))
        {
            throw new ArgumentException("User is not a customer.");
        }

        // Check vehicle availability
        var isAvailable = await _rentalRepository.IsVehicleAvailableAsync(
            vehicleId, startDate, endDate);
        if (!isAvailable)
        {
            throw new InvalidOperationException(
                "Vehicle is not available for the selected dates.");
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle == null)
        {
            throw new ArgumentException("Vehicle not found.");
        }

        // Calculate price using the selected strategy
        var price = await CalculatePriceAsync(
            vehicleId, userId, startDate, endDate, pricingStrategy);

        var rental = new Rental
        {
            UserId = userId,  // Changed from CustomerId
            VehicleId = vehicleId,
            StartDate = startDate,
            EndDate = endDate,
            TotalCost = price,
            Status = RentalStatus.Reserved,
            CreatedAt = DateTime.UtcNow
        };

        await _rentalRepository.AddAsync(rental);
        await _unitOfWork.CommitAsync();

        return rental;
    }
}
```

### Phase 7: Update Frontend

#### Update Models

**File**: `Frontend/Models/CustomerDtos.cs` ? **RENAME TO** `Frontend/Models/UserDtos.cs`

```csharp
namespace Frontend.Models;

public enum CustomerTier
{
    Standard,
    Silver,
    Gold,
    Platinum
}

public class UserProfile
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
}
```

#### Update ApiService

**File**: `Frontend/Services/ApiService.cs`

**Changes:**
```csharp
public interface IApiService
{
    // Users (changed from Customers)
    Task<List<UserProfile>> GetCustomersAsync(); // Returns users with Customer role
    Task<UserProfile?> GetCurrentUserProfileAsync(); // Changed from GetCurrentCustomerAsync
    Task<UserProfile?> GetMyProfileAsync();
    Task<bool> UpdateMyProfileAsync(UpdateProfileRequest request);
    
    // Rentals
    Task<List<Rental>> GetMyRentalsAsync();
    Task<Rental?> CreateRentalAsync(CreateRentalRequest request);
    // ...
}

public class ApiService : IApiService
{
    // Replace /api/customers with /api/users
    public async Task<UserProfile?> GetCurrentUserProfileAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserProfile>("api/users/me");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching current user profile: {ex.Message}");
            return null;
        }
    }

    public async Task<List<UserProfile>> GetCustomersAsync()
    {
        try
        {
            var users = await _httpClient.GetFromJsonAsync<List<UserProfile>>(
                "api/users/customers");
            return users ?? new List<UserProfile>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching customers: {ex.Message}");
            return new List<UserProfile>();
        }
    }
}
```

#### Update Pages

**Files to Update:**
- `Frontend/Pages/BookVehicle.razor`
- `Frontend/Pages/Profile.razor`
- `Frontend/Pages/MyRentals.razor`
- `Frontend/Pages/CreateRental.razor`
- `Frontend/Pages/Customers.razor`

**Example Change in BookVehicle.razor:**
```csharp
@code {
    private UserProfile? currentUser; // Changed from Customer

    protected override async Task OnInitializedAsync()
    {
        // Load user profile
        currentUser = await ApiService.GetCurrentUserProfileAsync();
        
        if (currentUser == null)
        {
            Snackbar.Add("Unable to load your profile.", Severity.Error);
            return;
        }
        
        // ... rest of code
    }
}
```

### Phase 8: Update Rental DTOs

**File**: `Backend/Application/DTOs/RentalDtos.cs`

```csharp
public class CreateRentalDto
{
    public string UserId { get; set; } = null!; // Changed from int CustomerId
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PricingStrategy { get; set; } = "standard";
}

public class CalculatePriceDto
{
    public int VehicleId { get; set; }
    public string UserId { get; set; } = null!; // Changed from int CustomerId
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PricingStrategy { get; set; } = "standard";
}
```

## Benefits of This Refactoring

### 1. **Eliminates Data Duplication** ?
- No more syncing between `AspNetUsers` and `Customers`
- Single source of truth for user data

### 2. **Fixes 404 Errors** ?
- No more missing Customer records
- All Customer role users automatically have profile data

### 3. **Simplifies Codebase** ?
- Fewer tables to manage
- Fewer repositories needed
- Simpler queries

### 4. **Better Performance** ?
- Fewer JOIN operations
- Faster queries

### 5. **Easier Maintenance** ?
- One place to update user information
- No sync issues between tables

## Migration Order

1. ? **Backup Database** (CRITICAL!)
2. ? Extend `ApplicationUser` entity
3. ? Update `Rental` entity
4. ? Update `DbContext`
5. ? Create and run migration
6. ? Update controllers
7. ? Update services
8. ? Update repositories
9. ? Update frontend models
10. ? Update frontend services
11. ? Update frontend pages
12. ? Test thoroughly
13. ? Delete `create_missing_customer_records.sql` (no longer needed)
14. ? Update documentation

## Rollback Plan

If issues occur:
1. Restore database backup
2. Revert code changes
3. Run previous migration: `dotnet ef database update [PreviousMigration]`

## Testing Checklist

After migration:
- [ ] Existing rentals still linked to correct users
- [ ] Customer login works
- [ ] Profile page loads
- [ ] Booking works
- [ ] My Rentals shows correct data
- [ ] Admin can see all customers
- [ ] Price calculation works
- [ ] Rental cancellation works
- [ ] No 404 errors on `/api/users/me`
- [ ] Tier system works
- [ ] Driver license validation works

## Estimated Time

- Development: 4-6 hours
- Testing: 2-3 hours
- **Total: 6-9 hours**

## Risk Assessment

**Risk Level**: ?? **MEDIUM-HIGH**

**Risks:**
- Data loss if migration fails
- Breaking existing rentals
- Frontend API calls fail
- Performance issues with UserManager

**Mitigation:**
- ? Full database backup before migration
- ? Test on development database first
- ? Gradual rollout
- ? Keep old endpoints temporarily
- ? Monitor logs carefully

---

**Status**: ?? **READY FOR IMPLEMENTATION**
**Priority**: ?? **MEDIUM** (Nice-to-have, not urgent)
**Breaking Changes**: ? **YES** (Major refactoring)
