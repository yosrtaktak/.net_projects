# IMPLEMENTATION GUIDE: Remove Customers Table

## ?? IMPORTANT: READ BEFORE PROCEEDING

This is a **MAJOR BREAKING CHANGE** that affects the entire application. 

### ? **Files Already Created/Modified:**
1. ? `Backend/Core/Entities/ApplicationUser.cs` - Extended with customer fields
2. ? `Backend/Core/Entities/Rental.cs` - Changed to use UserId instead of CustomerId
3. ? `Backend/Infrastructure/Data/CarRentalDbContext.cs` - Updated configuration
4. ? `Backend/Migrations/20241202000000_RemoveCustomersTable.cs` - Migration file
5. ? `Backend/Application/DTOs/UserDtos.cs` - New user profile DTOs
6. ? `Backend/Application/DTOs/RentalDtos.cs` - Updated to use UserId
7. ? `Backend/Controllers/UsersController.cs` - New controller to replace CustomersController

### ?? **Files That Still Need Updates:**
Due to the large number of files, I recommend a **phased approach**:

## Phase 1: Backend Core (COMPLETE ?)
- [x] ApplicationUser entity
- [x] Rental entity  
- [x] DbContext
- [x] Migration
- [x] User DTOs
- [x] UsersController

## Phase 2: Backend Services & Repositories (TODO)
- [ ] IRentalService interface
- [ ] RentalService implementation
- [ ] IRentalRepository interface
- [ ] RentalRepository implementation
- [ ] ICustomerRepository (DELETE or update)
- [ ] CustomerRepository (DELETE or update)
- [ ] Update pricing strategies
- [ ] Update report service

## Phase 3: Backend Controllers (TODO)
- [ ] RentalsController - Update to use UserId
- [ ] AuthController - No Customer record creation needed
- [ ] ReportsController - Update queries
- [ ] Delete or keep CustomersController for backward compatibility

## Phase 4: Frontend Models & Services (TODO)
- [ ] Rename CustomerDtos.cs to UserDtos.cs
- [ ] Update all model references
- [ ] Update ApiService methods
- [ ] Update all API endpoint calls

## Phase 5: Frontend Pages (TODO)
- [ ] BookVehicle.razor
- [ ] Profile.razor
- [ ] MyRentals.razor
- [ ] CreateRental.razor
- [ ] Customers.razor ? Users.razor
- [ ] CustomerDetails.razor ? UserDetails.razor
- [ ] All rental-related pages

## Step-by-Step Implementation

### BEFORE YOU START:
1. ? **BACKUP YOUR DATABASE**
2. ? **Create a new git branch**: `git checkout -b refactor-remove-customers-table`
3. ? **Commit current work**: `git commit -am "Backup before refactoring"`

### Step 1: Apply Backend Changes (1-2 hours)

**1.1 Service Layer Updates**

You need to update all services that use Customer to use ApplicationUser:

**File: `Backend/Application/Services/RentalService.cs`**

Key changes needed:
- Add `UserManager<ApplicationUser>` dependency
- Change method signatures from `int customerId` to `string userId`
- Replace `Customer` lookups with `UserManager.FindByIdAsync(userId)`
- Update pricing calculations to use `ApplicationUser`

**File: `Backend/Core/Interfaces/IRentalService.cs`**
```csharp
Task<Rental> CreateRentalAsync(string userId, int vehicleId, DateTime startDate, DateTime endDate, string pricingStrategy = "standard");
Task<IEnumerable<Rental>> GetRentalsByUserAsync(string userId);
Task<decimal> CalculatePriceAsync(int vehicleId, string userId, DateTime startDate, DateTime endDate, string pricingStrategy = "standard");
```

**1.2 Repository Updates**

**File: `Backend/Core/Interfaces/IRentalRepository.cs`**
```csharp
Task<IEnumerable<Rental>> GetRentalsByUserIdAsync(string userId);
Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startDate, DateTime endDate);
```

**File: `Backend/Infrastructure/Repositories/RentalRepository.cs`**
```csharp
public async Task<IEnumerable<Rental>> GetRentalsByUserIdAsync(string userId)
{
    return await _context.Rentals
        .Include(r => r.Vehicle)
        .Include(r => r.User)
        .Include(r => r.Payment)
        .Where(r => r.UserId == userId)
        .OrderByDescending(r => r.CreatedAt)
        .ToListAsync();
}
```

**1.3 Controller Updates**

**File: `Backend/Controllers/RentalsController.cs`**

Major changes:
- Inject `UserManager<ApplicationUser>`
- Change `CustomerId` to `UserId` everywhere
- Update all service calls to use `string userId` instead of `int customerId`

Example:
```csharp
private readonly UserManager<ApplicationUser> _userManager;

[HttpPost]
public async Task<ActionResult<Rental>> CreateRental([FromBody] CreateRentalDto dto)
{
    try
    {
        string userId;
        
        if (User.IsInRole("Customer"))
        {
            var userEmail = User.Identity?.Name;
            var user = await _userManager.FindByEmailAsync(userEmail);
            userId = user.Id;
        }
        else
        {
            userId = dto.UserId;
        }

        var rental = await _rentalService.CreateRentalAsync(
            userId, dto.VehicleId, dto.StartDate, dto.EndDate, dto.PricingStrategy);

        return CreatedAtAction(nameof(GetRental), new { id = rental.Id }, rental);
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}

[HttpGet("user/{userId}")]
public async Task<ActionResult<IEnumerable<Rental>>> GetRentalsByUser(string userId)
{
    var rentals = await _rentalService.GetRentalsByUserAsync(userId);
    return Ok(rentals);
}
```

### Step 2: Run Migration (30 minutes)

**2.1 Stop Backend**
```powershell
# Press Ctrl+C in backend terminal
```

**2.2 Apply Migration**
```powershell
cd Backend
dotnet ef database update
```

**Expected Output:**
```
Applying migration '20241202000000_RemoveCustomersTable'.
Done.
```

**2.3 Verify Migration**
```sql
-- Check AspNetUsers has new columns
SELECT TOP 1 * FROM AspNetUsers;

-- Should show: DriverLicenseNumber, DateOfBirth, Address, RegistrationDate, Tier

-- Check Rentals uses UserId
SELECT TOP 1 * FROM Rentals;

-- Should show: UserId (nvarchar), NOT CustomerId

-- Check Customers table is gone
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Customers';

-- Should return empty (table doesn't exist)
```

### Step 3: Update Frontend (2-3 hours)

**3.1 Update Models**

**File: `Frontend/Models/UserDtos.cs`** (rename from CustomerDtos.cs)
```csharp
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

**3.2 Update ApiService**

**File: `Frontend/Services/ApiService.cs`**

Replace all `/api/customers` endpoints with `/api/users`:

```csharp
// Old: await _httpClient.GetFromJsonAsync<Customer>("api/customers/me");
// New:
public async Task<UserProfile?> GetCurrentUserProfileAsync()
{
    return await _httpClient.GetFromJsonAsync<UserProfile>("api/users/me");
}

// Old: await _httpClient.GetFromJsonAsync<List<Customer>>("api/customers");
// New:
public async Task<List<UserProfile>> GetAllCustomersAsync()
{
    return await _httpClient.GetFromJsonAsync<List<UserProfile>>("api/users/customers") 
        ?? new List<UserProfile>();
}
```

**3.3 Update Pages**

Global find/replace (use with caution):
- `Customer?` ? `UserProfile?`
- `Customer ` ? `UserProfile `
- `new Customer` ? `new UserProfile`
- `GetCurrentCustomerAsync` ? `GetCurrentUserProfileAsync`
- `GetCustomersAsync` ? `GetAllCustomersAsync`
- `api/customers` ? `api/users`

**Files to update:**
- ? `Frontend/Pages/BookVehicle.razor`
- ? `Frontend/Pages/Profile.razor`
- ? `Frontend/Pages/MyRentals.razor`
- ? `Frontend/Pages/CreateRental.razor`
- ? `Frontend/Pages/Customers.razor`
- ? `Frontend/Pages/CustomerDetails.razor`

### Step 4: Testing (1-2 hours)

**4.1 Backend Tests**
```bash
# Test new endpoints
curl -X GET https://localhost:5000/api/users/me -H "Authorization: Bearer {TOKEN}"
curl -X GET https://localhost:5000/api/users/customers -H "Authorization: Bearer {TOKEN}"
```

**4.2 Integration Tests**
- [ ] Register new customer account
- [ ] Login as customer
- [ ] View profile at `/profile`
- [ ] Browse vehicles at `/vehicles/browse`
- [ ] Book a vehicle
- [ ] View "My Rentals"
- [ ] Cancel a rental
- [ ] Login as admin
- [ ] View all customers at `/customers`
- [ ] Create rental for customer
- [ ] View reports

### Step 5: Cleanup

**5.1 Delete Obsolete Files**
```bash
rm Backend/Controllers/CustomersController.cs
rm Backend/Core/Entities/Customer.cs
rm Backend/Core/Interfaces/ICustomerRepository.cs
rm Backend/Infrastructure/Repositories/CustomerRepository.cs
rm Backend/create_missing_customer_records.sql
```

**5.2 Update Documentation**
- Update README files
- Update API documentation
- Update testing guides

## Rollback Procedure

If something goes wrong:

```powershell
# 1. Revert migration
cd Backend
dotnet ef database update [PreviousMigrationName]

# 2. Revert code
git reset --hard HEAD~1

# 3. Restart backend
dotnet run
```

## Common Issues & Solutions

### Issue: "Column 'CustomerId' does not exist"
**Cause**: Migration not applied
**Solution**: Run `dotnet ef database update`

### Issue: "Cannot insert NULL into UserId"
**Cause**: Data migration failed
**Solution**: Check migration SQL, ensure all Customers have matching AspNetUsers

### Issue: Frontend 404 errors
**Cause**: Still calling old `/api/customers` endpoints
**Solution**: Update all API calls to `/api/users`

### Issue: "User not found" errors
**Cause**: UserId (string) vs CustomerId (int) mismatch
**Solution**: Ensure all DTOs and service methods use `string userId`

## Estimated Timeline

| Phase | Time | Complexity |
|-------|------|-----------|
| Backend Core | 1-2 hours | Medium |
| Backend Services | 2-3 hours | High |
| Migration | 30 mins | Low |
| Frontend | 2-3 hours | Medium |
| Testing | 1-2 hours | Medium |
| **Total** | **7-11 hours** | **High** |

## Decision Point

### Option A: Full Refactoring (Recommended for Long Term)
**Pros:**
- ? Cleaner architecture
- ? No data duplication
- ? Easier maintenance
- ? Better performance

**Cons:**
- ? High effort (7-11 hours)
- ? Breaking changes
- ? Risk of bugs
- ? Requires thorough testing

### Option B: Keep Current Architecture (Quick Fix)
**Pros:**
- ? Zero effort
- ? No breaking changes
- ? Works today

**Cons:**
- ? Continues data duplication
- ? Sync issues persist
- ? 404 errors require SQL script
- ? Technical debt

### Option C: Hybrid (Keep Both, Sync Automatically)
**Pros:**
- ? Backward compatible
- ? Gradual migration
- ? Lower risk

**Cons:**
- ? More complex
- ? Still duplicates data
- ? Temporary solution

## Recommendation

**For Production System**: Option C (Hybrid) first, then Option A
**For New/Small System**: Option A (Full Refactoring)
**If Time-Constrained**: Option B (Keep Current) + Run SQL script

## What I've Done So Far

? Created all core backend changes
? Created migration script
? Created new UsersController
? Updated DTOs
? Created comprehensive documentation

## What You Need to Do

1. **Review the changes** in the 7 files I modified/created
2. **Decide** which option (A, B, or C) to pursue
3. **If Option A**: Continue with remaining implementation steps
4. **If Option B**: Revert my changes and run `create_missing_customer_records.sql`
5. **If Option C**: Keep Customers table but auto-sync with AspNetUsers

Let me know which approach you'd like to take, and I can help implement it!

---

**Status**: ?? **PAUSED - AWAITING DECISION**
**Files Modified**: 7
**Files Remaining**: ~30+
**Estimated Completion**: 7-11 hours
