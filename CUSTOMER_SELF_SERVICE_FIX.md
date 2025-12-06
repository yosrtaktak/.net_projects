# Customer Self-Service and Damage Reporting Fix

## Issues Fixed

### 1. ? 404 Error on `/api/customers/me`
**Problem**: Customers getting 404 when trying to access their profile.

**Root Cause**: The endpoint exists on backend, but the customer record might not exist in the database for the authenticated user.

**Solution**: 
- Added `GetCurrentCustomerAsync()` method to `IApiService` interface
- Implemented the method in `ApiService.cs` to call `/api/customers/me`
- Added error handling and logging for better debugging

### 2. ? "Rent Now" Button Redirects to Home
**Problem**: When customers clicked "Rent Now" on a vehicle, they were redirected to home instead of being able to complete the rental.

**Root Cause**: `CreateRental.razor` was using `AdminLayout` and checking for Admin/Employee roles only, blocking customers.

**Solution**: **Completely rewrote `CreateRental.razor` to support both customer self-service and admin/employee rental creation:**

#### Dynamic Layout Selection
```razor
@if (isCustomer)
{
    <DynamicComponent Type="@typeof(CustomerLayout)" Parameters="@layoutParameters">
        @RenderContent()
    </DynamicComponent>
}
else
{
    <DynamicComponent Type="@typeof(AdminLayout)" Parameters="@layoutParameters">
        @RenderContent()
    </DynamicComponent>
}
```

#### Role-Based Features
- **Customers**: 
  - Automatically use their own customer ID
  - See simplified booking interface
  - Navigate back to Browse Vehicles
  - After booking, redirect to `/my-rentals`
  
- **Admin/Employee**:
  - Can select any customer from dropdown
  - Full management interface
  - Navigate back to Manage Rentals
  - After creation, redirect to `/rentals/manage`

#### Key Changes in CreateRental.razor
1. **Removed layout attribute** - Now uses dynamic layout based on role
2. **Auto-load customer profile** for customers using `GetCurrentCustomerAsync()`
3. **Conditional customer selection** - Only show dropdown for admin/employee
4. **Role-based navigation** - Different back buttons and redirect paths
5. **Form validation** - Different validation logic for customers vs admin

### 3. ? Missing Accident/Damage Reporting for Customers
**Problem**: Customers couldn't report accidents or damage during their rentals.

**Solution**: **Enhanced existing pages and added API methods:**

#### Added API Methods to ApiService.cs
```csharp
// Vehicle Damages
Task<List<VehicleDamage>> GetVehicleDamagesAsync();
Task<VehicleDamage?> GetVehicleDamageAsync(int id);
Task<List<VehicleDamage>> GetDamagesByRentalAsync(int rentalId);
Task<VehicleDamage?> CreateVehicleDamageAsync(CreateVehicleDamageRequest request);
Task<List<VehicleDamage>> GetMyDamagesAsync();
```

#### Updated ReportDamage.razor
- Fixed to use `ApiService.CreateVehicleDamageAsync()` instead of `ApiServiceExtensions`
- Added proper `CreateVehicleDamageRequest` model usage
- Removed unnecessary `HttpClient` injection
- Auto-calculates repair costs based on severity if not provided:
  - Minor: $100
  - Moderate: $300
  - Major: $800
  - Critical: $2,000

#### Updated MyRentals.razor
- Fixed `ViewDamageReports()` to use `ApiService.GetDamagesByRentalAsync()`
- Removed unnecessary `HttpClient` and `ApiServiceExtensions` usage
- Shows "Report Damage" button for Active and Completed rentals
- Shows "View Reports" button to see existing damage reports
- Added damage reports dialog with severity and status indicators

#### Added Frontend Models
Added to `VehicleDamageModels.cs`:
```csharp
public class VehicleDamage
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public int? RentalId { get; set; }
    public Rental? Rental { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Description { get; set; }
    public DamageSeverity Severity { get; set; }
    public decimal RepairCost { get; set; }
    public DateTime? RepairedDate { get; set; }
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
    public DamageStatus Status { get; set; }
}

public class CreateVehicleDamageRequest
{
    public int VehicleId { get; set; }
    public int? RentalId { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Description { get; set; }
    public int Severity { get; set; }
    public decimal RepairCost { get; set; }
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
}
```

## Files Modified

### Frontend Files
1. **Frontend/Pages/CreateRental.razor** - Complete rewrite for dual-role support
2. **Frontend/Services/ApiService.cs** - Added damage reporting methods and `GetCurrentCustomerAsync()`
3. **Frontend/Models/VehicleDamageModels.cs** - Added `VehicleDamage` and `CreateVehicleDamageRequest` models
4. **Frontend/Pages/ReportDamage.razor** - Updated to use ApiService methods
5. **Frontend/Pages/MyRentals.razor** - Updated to use ApiService methods for damage reports

### Backend Files
No backend changes required - the endpoints already existed and were working correctly.

## Customer User Flow

### Renting a Vehicle
1. Customer logs in
2. Browses vehicles at `/vehicles/browse`
3. Clicks "Rent Now" on available vehicle
4. Redirected to `/rentals/create?vehicleId=X`
5. Page automatically loads customer profile
6. Customer selects dates and pricing strategy
7. Sees price calculation in real-time
8. Clicks "Book Now"
9. Redirected to `/my-rentals` to see their new booking

### Reporting Damage
1. Customer goes to "My Rentals" at `/my-rentals`
2. Sees their Active or Completed rentals
3. Clicks "Report Damage" button
4. Redirected to `/rentals/{rentalId}/report-damage`
5. Fills out damage form:
   - Severity (Minor, Moderate, Major, Critical)
   - Description (required)
   - Estimated cost (optional)
   - Image URL (optional)
6. Clicks "Submit Damage Report"
7. Report is sent to admin for review
8. Redirected back to `/my-rentals`

### Viewing Damage Reports
1. Customer goes to "My Rentals"
2. Clicks "View Reports" button on any rental
3. Dialog opens showing all damage reports for that rental
4. Shows: Severity, Status, Description, Cost, Reporter, Dates

## Backend Authorization (Already Implemented)

### CustomersController
- `GET /api/customers/me` - Authenticated customers can get their own profile
- `GET /api/customers/me/rentals` - Customers can get their own rentals
- `GET /api/customers/me/damages` - Customers can get their damage reports

### RentalsController
- `POST /api/rentals` - Customers can create rentals (customer ID auto-set from token)
- `POST /api/rentals/calculate-price` - Customers can calculate rental prices
- `PUT /api/rentals/{id}/cancel` - Customers can cancel their own reservations

### VehicleDamagesController
- `POST /api/vehicledamages` - Customers can report damage for their rentals
- `GET /api/vehicledamages/rental/{id}` - Customers can view damages for their rentals
- `GET /api/vehicledamages/{id}` - Customers can view specific damage if it's from their rental

## Testing Checklist

### Customer Rental Flow
- [ ] Customer can browse vehicles
- [ ] Click "Rent Now" navigates to create rental page
- [ ] Customer profile auto-loads
- [ ] Vehicle pre-selected from query parameter
- [ ] Price calculates when dates selected
- [ ] Can select pricing strategy
- [ ] "Book Now" creates rental successfully
- [ ] Redirects to My Rentals after booking
- [ ] New rental appears in My Rentals list

### Damage Reporting
- [ ] "Report Damage" button visible on Active/Completed rentals
- [ ] Clicking opens report damage page
- [ ] Rental info displayed correctly
- [ ] Can select severity level
- [ ] Can enter description
- [ ] Optional fields work correctly
- [ ] Auto-calculates repair cost if not provided
- [ ] Submit creates damage report
- [ ] Redirects to My Rentals
- [ ] "View Reports" shows damage reports in dialog

### API Endpoints
- [ ] `GET /api/customers/me` returns customer profile
- [ ] `GET /api/customers/me/rentals` returns customer's rentals
- [ ] `GET /api/customers/me/damages` returns customer's damage reports
- [ ] `POST /api/rentals` creates rental with customer ID from token
- [ ] `POST /api/vehicledamages` creates damage report
- [ ] `GET /api/vehicledamages/rental/{id}` returns damages for customer's rental

## Database Requirements

### Customer Account Required
For the `/api/customers/me` endpoint to work, the authenticated user must have a corresponding record in the `Customers` table where `Email` matches the JWT token's `User.Identity.Name`.

**If 404 persists:**
1. Check that the customer exists in database: 
   ```sql
   SELECT * FROM Customers WHERE Email = 'user@example.com';
   ```

2. Check JWT token contains email:
   - Decode JWT at jwt.io
   - Verify `User.Identity.Name` claim exists

3. Ensure customer is created during registration:
   - Check `AuthController.Register` method
   - Verify it creates a Customer record along with User account

## Benefits

? **Customers can now:**
- Book vehicles independently without admin assistance
- Report damage during or after their rentals
- View their damage report history
- Cancel their own reservations
- Calculate rental prices before booking

? **Admin workflow preserved:**
- Can still create rentals for customers
- Can view and manage all rentals
- Can handle damage reports
- Full management capabilities retained

? **Better user experience:**
- Role-appropriate interfaces
- Clear navigation paths
- Real-time price calculations
- Damage tracking and transparency

## Next Steps

1. **Test the customer flow end-to-end**
2. **Verify customer records exist** for all user accounts
3. **Test damage reporting** with different severity levels
4. **Check authorization** works correctly for all endpoints
5. **Monitor logs** for any authentication issues

## Support

If issues persist:
1. Check browser console for errors
2. Check backend logs for API errors
3. Verify JWT token is valid and contains correct claims
4. Ensure customer record exists in database
5. Check network tab for actual API responses
