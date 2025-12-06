# Refactoring Progress Report: Customer to ApplicationUser Migration

## ?? Status: IN PROGRESS (Backend ~90% Complete, Frontend ~30% Complete)

---

## ? **COMPLETED - Backend Files**

### 1. **Core Entities & Interfaces**
- ? `Backend/Core/Entities/ApplicationUser.cs` - Extended with customer fields
- ? `Backend/Core/Entities/Rental.cs` - Updated to use UserId (string)
- ? `Backend/Core/Interfaces/IPricingStrategy.cs` - Updated to use ApplicationUser
- ? `Backend/Core/Interfaces/IRentalRepository.cs` - Already uses string userId

### 2. **Pricing Strategies**
- ? `Backend/Application/Services/PricingStrategies/StandardPricingStrategy.cs`
- ? `Backend/Application/Services/PricingStrategies/LoyaltyPricingStrategy.cs`
- ? `Backend/Application/Services/PricingStrategies/WeekendPricingStrategy.cs`
- ? `Backend/Application/Services/PricingStrategies/SeasonalPricingStrategy.cs`

### 3. **Repositories**
- ? `Backend/Infrastructure/Repositories/RentalRepository.cs` - Updated to use User/UserId

### 4. **Services**
- ? `Backend/Application/Services/RentalService.cs` - Already updated in context file
- ? `Backend/Application/Services/ReportService.cs` - Updated to use UserManager

### 5. **Controllers**
- ? `Backend/Controllers/AuthController.cs` - Removed Customer record creation
- ? `Backend/Controllers/RentalsController.cs` - Updated to use UserManager & UserId
- ? `Backend/Controllers/UsersController.cs` - New controller created

### 6. **DTOs**
- ? `Backend/Application/DTOs/RentalDtos.cs` - Uses string UserId
- ? `Backend/Application/DTOs/UserDtos.cs` - New user DTOs created

### 7. **Database & Configuration**
- ? `Backend/Infrastructure/Data/CarRentalDbContext.cs` - Updated configuration
- ? `Backend/Migrations/20241202000000_RemoveCustomersTable.cs` - Migration created

---

## ? **COMPLETED - Frontend Files**

### 1. **Models**
- ? `Frontend/Models/CustomerDtos.cs` - Added UserProfile with string Id + Customer alias
- ? `Frontend/Models/RentalDtos.cs` - Updated to use string UserId with backward compatibility

### 2. **Services**
- ? `Frontend/Services/ApiService.cs` - Updated customer endpoints to /api/users
  - Changed `GetCurrentCustomerAsync()` to use `/api/users/me`
  - Changed `GetCustomersAsync()` to use `/api/users/customers`
  - Updated `GetMyRentalsAsync()` to use `/api/rentals/user/{userId}`
  - Changed `GetRentalsForManagementAsync()` parameter from `int? customerId` to `string? userId`

---

## ? **TODO - Frontend Pages (Build Errors to Fix)**

### Critical Errors (Must Fix):

#### 1. **`Frontend/Pages/Customers.razor`** - Line 165
```csharp
// ERROR: Cannot convert from 'string' to 'int'
ViewCustomerDetails(customerId)  // customerId is now string, not int

// FIX: Update method signature and navigation
private void ViewCustomerDetails(string userId)
{
    NavigationManager.NavigateTo($"/customers/{userId}");
}
```

#### 2. **`Frontend/Pages/CustomerDetails.razor`** - Line 121
```csharp
// ERROR: Cannot convert from 'DateTime?' to 'DateTime'
// ERROR: No overload for method 'ToString' takes 1 arguments

// FIX: Handle nullable DateTime properly
@customer.DateOfBirth?.ToString("MMM dd, yyyy") ?? "N/A"
@customer.RegistrationDate.ToString("MMM dd, yyyy")
```

#### 3. **`Frontend/Pages/BookVehicle.razor`** - Lines 407, 455
```csharp
// ERROR: Cannot implicitly convert type 'string' to 'int'
var request = new CalculatePriceRequest
{
    VehicleId = selectedVehicleId,
    UserId = currentCustomer.Id,  // FIX: Use UserId not CustomerId
    StartDate = rentalRequest.StartDate,
    EndDate = rentalRequest.EndDate,
    PricingStrategy = rentalRequest.PricingStrategy
};

var request = new CreateRentalRequest
{
    UserId = currentCustomer.Id,  // FIX: Use UserId not CustomerId
    VehicleId = selectedVehicleId,
    StartDate = startDate.Value,
    EndDate = endDate.Value,
    PricingStrategy = selectedPricingStrategy
};
```

#### 4. **`Frontend/Pages/Profile.razor`** - Line 110
```csharp
// ERROR: No overload for method 'ToString' takes 1 arguments

// FIX: Handle nullable DateTime
@customer.DateOfBirth?.ToString("MMM dd, yyyy") ?? "Not provided"
```

#### 5. **`Frontend/Pages/CreateRental.razor`**
```csharp
// Multiple locations need updating:
// 1. Change all CustomerId references to UserId
// 2. Update CalculatePriceRequest to use UserId
// 3. Update CreateRentalRequest to use UserId
```

---

## ?? **Additional Frontend Pages to Update**

These pages may also need updates (check for Customer references):

- `Frontend/Pages/MyRentals.razor`
- `Frontend/Pages/RentalHistory.razor`
- `Frontend/Pages/ManageRentals.razor` (if exists)
- `Frontend/Pages/Reports.razor` (customer display)
- `Frontend/Pages/AdminDashboard.razor` (customer counts/display)

---

## ?? **Quick Fix Guide**

### Step 1: Fix Critical Build Errors

Update these files in order:

1. **Customers.razor**
```csharp
// Change:
private void ViewCustomerDetails(int customerId)
// To:
private void ViewCustomerDetails(string userId)
{
    NavigationManager.NavigateTo($"/customers/{userId}");
}

// Update the call site too - customer.Id is now string
```

2. **CustomerDetails.razor**
```csharp
// Add parameter:
[Parameter]
public string? Id { get; set; }  // Changed from int Id

// Fix DateTime displays:
@customer.DateOfBirth?.ToString("MMM dd, yyyy") ?? "N/A"
```

3. **BookVehicle.razor**
```csharp
// Change all instances of:
CustomerId = currentCustomer.Id
// To:
UserId = currentCustomer.Id

// The requests should use UserId property, not CustomerId
```

4. **Profile.razor**
```csharp
// Fix DateTime display:
<MudText>@customer.DateOfBirth?.ToString("MMM dd, yyyy") ?? "Not provided"</MudText>
```

5. **CreateRental.razor**
```csharp
// Update:
rentalRequest.UserId = value;  // not CustomerId
await CalculatePrice();

// In CalculatePrice():
UserId = rentalRequest.UserId,  // not CustomerId

// In HandleCreateRental():
UserId = rentalRequest.UserId,  // not CustomerId
```

---

## ??? **Database Migration**

### ?? **IMPORTANT: Do NOT run migration until frontend is fixed!**

The migration is ready but should only be run after:
1. All build errors are fixed
2. Code compiles successfully
3. You've tested the changes

### When Ready to Migrate:

```bash
# 1. BACKUP DATABASE FIRST!
# 2. Then run:
cd Backend
dotnet ef database update
```

---

## ?? **Progress Summary**

| Category | Status | Complete |
|----------|--------|----------|
| **Backend Core** | ? Done | 100% |
| **Backend Services** | ? Done | 100% |
| **Backend Controllers** | ? Done | 100% |
| **Backend DTOs** | ? Done | 100% |
| **Frontend Models** | ? Done | 100% |
| **Frontend Services** | ? Done | 100% |
| **Frontend Pages** | ? In Progress | ~30% |
| **Database Migration** | ?? Ready | N/A |

**Overall Progress: ~75% Complete**

---

## ?? **Next Steps**

1. ? Fix the 5 critical build errors listed above
2. ? Test compilation: `cd Backend; dotnet build`
3. ? Review all frontend pages for Customer/CustomerId references
4. ? Update any remaining pages
5. ? Test the application thoroughly
6. ? Run database migration
7. ? Delete obsolete files:
   - `Backend/Core/Entities/Customer.cs`
   - `Backend/Controllers/CustomersController.cs`
   - `Backend/Core/Interfaces/ICustomerRepository.cs`
   - `Backend/Infrastructure/Repositories/CustomerRepository.cs`

---

## ?? **Benefits After Completion**

- ? No more data duplication between AspNetUsers and Customers
- ? No more 404 errors from missing Customer records
- ? Simpler codebase with fewer tables
- ? Better performance (fewer JOINs)
- ? Easier maintenance
- ? Single source of truth for user data

---

## ?? **Need Help?**

The refactoring is mostly complete! Just need to fix the frontend build errors.

**Estimated Time to Complete:** 30-60 minutes for remaining fixes.

