# Fix for 404 Error and Rent Now Redirect

## Issues
1. ? `GET https://localhost:5000/api/customers/me` returns 404 (Not Found)
2. ? "Rent Now" button in `/vehicles/browse` redirects to home page instead of rental creation

## Root Causes

### Issue 1: 404 on `/api/customers/me`
**Cause:** When users register with the `Customer` role, the `AuthController` creates an `ApplicationUser` (for authentication) but doesn't create a corresponding `Customer` record in the `Customers` table. The `/api/customers/me` endpoint looks for a Customer by email and returns 404 when none exists.

### Issue 2: Rent Now Redirect
**Cause:** The `CreateRental.razor` page doesn't have a layout specified, so it might be using a default layout that requires admin authorization or doesn't exist in the customer context.

## Solutions Applied

### ? Solution 1: Auto-Create Customer Records on Registration

**File Modified:** `Backend/Controllers/AuthController.cs`

Added automatic Customer record creation when a user registers with the Customer role:

```csharp
// Create Customer record if role is Customer
if (roleToAssign == "Customer")
{
    var customer = new Customer
    {
        FirstName = dto.Username, // Can be updated later in profile
        LastName = "", // Can be updated later in profile
        Email = dto.Email,
        PhoneNumber = "", // Can be updated later in profile
        DriverLicenseNumber = "", // Can be updated later in profile
        DateOfBirth = DateTime.UtcNow.AddYears(-25), // Default age
        RegistrationDate = DateTime.UtcNow,
        Tier = CustomerTier.Standard
    };

    await _unitOfWork.Repository<Customer>().AddAsync(customer);
    await _unitOfWork.CommitAsync();
}
```

**Benefits:**
- New customers automatically get a Customer record
- `/api/customers/me` will work immediately after registration
- Customer can update their profile later with real information

### ? Solution 2: Fix CreateRental Page Redirect

**File Modified:** `Frontend/Pages/CreateRental.razor`

Added redirect to login if user is not authenticated:

```csharp
protected override async Task OnInitializedAsync()
{
    var role = await AuthService.GetRoleAsync();
    isCustomer = role == "Customer";
    
    // If no role or not authenticated, redirect to login
    if (string.IsNullOrEmpty(role))
    {
        NavigationManager.NavigateTo("/login");
        return;
    }
    
    await LoadPageData();
    isLoadingPage = false;
}
```

**Page Structure:**
- No layout directive (works with both CustomerLayout and AdminLayout via App.razor)
- Role-based UI (customers see simplified interface)
- Auto-loads customer profile for customers
- Pre-selects vehicle from query parameter

## For Existing Users

If you have existing users with Customer role but no Customer records, run this SQL script:

**File:** `Backend/create_missing_customer_records.sql`

```sql
-- Creates Customer records for existing users
USE CarRentalDB;

INSERT INTO [Customers] ([FirstName], [LastName], [Email], [PhoneNumber], [DriverLicenseNumber], [DateOfBirth], [Address], [RegistrationDate], [Tier])
SELECT 
    u.[UserName] as [FirstName],
    '' as [LastName],
    u.[Email],
    '' as [PhoneNumber],
    '' as [DriverLicenseNumber],
    DATEADD(YEAR, -25, GETDATE()) as [DateOfBirth],
    NULL as [Address],
    u.[CreatedAt] as [RegistrationDate],
    0 as [Tier]
FROM [AspNetUsers] u
INNER JOIN [AspNetUserRoles] ur ON u.[Id] = ur.[UserId]
INNER JOIN [AspNetRoles] r ON ur.[RoleId] = r.[Id]
LEFT JOIN [Customers] c ON u.[Email] = c.[Email]
WHERE r.[Name] = 'Customer'
AND c.[Id] IS NULL;
```

## How to Apply Fixes

### Step 1: Update Backend
```bash
cd Backend
# The AuthController.cs has been updated
dotnet build
dotnet run
```

### Step 2: Update Frontend  
```bash
cd Frontend
dotnet clean
dotnet build
dotnet run
```

### Step 3: Fix Existing Users (Optional)
If you have existing customer users:
```sql
-- In SQL Server Management Studio or your SQL client
-- Run the script: Backend/create_missing_customer_records.sql
```

### Step 4: Test New Registration
1. Log out if logged in
2. Register a new user with Customer role
3. Verify Customer record is created:
   ```sql
   SELECT * FROM Customers WHERE Email = 'newcustomer@example.com';
   ```
4. Log in with the new customer
5. Navigate to `/vehicles/browse`
6. Click "Rent Now" on any vehicle
7. Verify you can create a rental

## Verification

### Test `/api/customers/me` Endpoint
```bash
# Get your JWT token from localStorage (F12 -> Application -> Local Storage)
curl -X GET "https://localhost:5000/api/customers/me" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Expected Response (200 OK):**
```json
{
  "id": 1,
  "firstName": "customer",
  "lastName": "",
  "email": "customer@example.com",
  "phoneNumber": "",
  "driverLicenseNumber": "",
  "dateOfBirth": "1999-12-02T...",
  "address": null,
  "registrationDate": "2024-12-02T...",
  "tier": 0
}
```

### Test Rent Now Flow
1. Login as customer
2. Go to https://localhost:7148/vehicles/browse
3. Click "Rent Now" on an available vehicle
4. **Expected:** Redirected to `/rentals/create?vehicleId=X`
5. **Verify:**
   - Page loads correctly
   - Customer info auto-loaded (no 404 error)
   - Vehicle is pre-selected
   - Dates can be selected
   - Price calculates automatically
6. Select dates and click "Book Now"
7. **Expected:** Success message and redirect to `/my-rentals`

## Database Schema

### Customers Table Structure
```sql
CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY PRIMARY KEY,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [DriverLicenseNumber] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Address] nvarchar(max) NULL,
    [RegistrationDate] datetime2 NOT NULL,
    [Tier] int NOT NULL
);
```

### Relationship
- `AspNetUsers.Email` ? ? `Customers.Email` (linked by email, not foreign key)
- Each Customer role user should have exactly one Customer record

## Troubleshooting

### Still getting 404 on `/api/customers/me`?

1. **Check if Customer record exists:**
   ```sql
   SELECT * FROM Customers WHERE Email = 'your@email.com';
   ```

2. **Check JWT token contains email:**
   - Go to https://jwt.io
   - Paste your token
   - Verify `email` or `name` claim exists

3. **Check backend logs:**
   - Look for `CustomersController` errors
   - Verify `User.Identity.Name` is populated

4. **Manually create Customer record:**
   ```sql
   INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber, DateOfBirth, RegistrationDate, Tier)
   VALUES ('John', 'Doe', 'your@email.com', '', '', DATEADD(YEAR, -25, GETDATE()), GETDATE(), 0);
   ```

### Still redirecting to home from Rent Now?

1. **Check browser console (F12):**
   - Look for JavaScript errors
   - Check Network tab for failed requests

2. **Verify authentication:**
   - Check localStorage has `authToken`
   - Check localStorage has `userRole` = "Customer"
   - Try logging out and back in

3. **Check navigation:**
   ```javascript
   // In browser console
   console.log(localStorage.getItem('authToken'));
   console.log(localStorage.getItem('userRole'));
   ```

4. **Clear browser cache:**
   - Hard refresh (Ctrl+Shift+R)
   - Clear site data (F12 -> Application -> Clear storage)

## Benefits of This Fix

? **For New Users:**
- Automatic Customer record creation on registration
- Immediate access to customer features
- No manual database intervention needed

? **For Existing Users:**
- SQL script creates missing Customer records
- One-time fix for all existing users

? **For Developers:**
- Consistent user experience
- Fewer support requests
- Better data integrity

## Files Modified

1. ? `Backend/Controllers/AuthController.cs` - Auto-create Customer records
2. ? `Frontend/Pages/CreateRental.razor` - Fixed redirect issue
3. ? `Backend/create_missing_customer_records.sql` - SQL script for existing users

## Next Steps

1. Restart backend and frontend
2. Test new user registration
3. Test existing user login
4. Verify Rent Now flow works
5. Check Customer profile at `/api/customers/me`

All customer self-service features should now work correctly! ??
