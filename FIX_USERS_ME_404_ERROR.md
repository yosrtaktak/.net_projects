# Fix: 404 Error on /api/users/me and Rental Creation

## Issues Identified

1. **404 Error on `/api/users/me`**: JWT token not providing the correct email claim for `User.Identity.Name`
2. **DriverLicenseNumber Column Error**: Database may be missing required columns in AspNetUsers table
3. **Customer Rental Creation Failing**: Cannot fetch user profile to create rentals

## Root Causes

### 1. JWT Token Claims Issue
The JWT token was setting `ClaimTypes.Name` to the username instead of the email. Since the `UsersController` uses `User.Identity?.Name` to get the email, this was returning the username instead, causing a lookup failure.

### 2. Database Schema Incomplete
The migration to ApplicationUser may not have been fully applied, missing columns like:
- FirstName
- LastName
- CreatedAt
- LastLogin
- DriverLicenseNumber
- DateOfBirth
- Address
- RegistrationDate
- Tier

## Solutions Applied

### Fix 1: Update JWT Token Generation

**File**: `Backend/Application/Services/JwtService.cs`

**Changes**:
- Changed `ClaimTypes.Name` to use email instead of username
- Added username as a custom claim
- This ensures `User.Identity.Name` returns the email address

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Name, user.Email ?? string.Empty), // Now uses email
    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
    new Claim("username", user.UserName ?? string.Empty) // Username as custom claim
};
```

### Fix 2: Improve UsersController Authentication

**File**: `Backend/Controllers/UsersController.cs`

**Changes**:
- Added fallback claim checking to handle multiple token formats
- Improved error messages for debugging
- Added `System.Security.Claims` namespace

```csharp
// Try multiple claim sources
var userEmail = User.FindFirst(ClaimTypes.Email)?.Value 
    ?? User.FindFirst(ClaimTypes.Name)?.Value
    ?? User.Identity?.Name;
```

### Fix 3: Database Column Fix Script

**File**: `Backend/fix_aspnetusers_columns.sql`

This script:
- Checks for all required columns in AspNetUsers
- Adds missing columns with appropriate defaults
- Verifies Rentals.UserId column and FK constraint
- Checks migration status
- Provides diagnostic information

## How to Apply the Fix

### Step 1: Run the Database Fix Script

**Option A: SQL Server Management Studio (SSMS)**
1. Open SSMS
2. Connect to your database server
3. Open file: `Backend/fix_aspnetusers_columns.sql`
4. Execute (F5)

**Option B: Command Line**
```powershell
cd Backend
sqlcmd -S localhost -d CarRentalDB -E -i fix_aspnetusers_columns.sql
```

**Option C: Azure Data Studio**
1. Open Azure Data Studio
2. Connect to your server
3. Create new query
4. Copy and paste script from `fix_aspnetusers_columns.sql`
5. Run

### Step 2: Rebuild and Restart Backend

```powershell
cd Backend
dotnet clean
dotnet build
dotnet run
```

**Expected Output**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5002
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Step 3: Clear Browser Cache and Re-login

**IMPORTANT**: Old JWT tokens won't have the correct claims format.

1. Open browser DevTools (F12)
2. Go to Application > Local Storage
3. Clear all items (especially `authToken`)
4. Close and reopen the browser
5. Login again to get a new JWT token

### Step 4: Verify the Fix

#### Test 1: Check Database Columns
```sql
USE CarRentalDB;

SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AspNetUsers'
AND COLUMN_NAME IN (
    'FirstName', 'LastName', 'DriverLicenseNumber', 
    'DateOfBirth', 'Address', 'RegistrationDate', 'Tier'
)
ORDER BY COLUMN_NAME;
```

**Expected**: All columns should be listed

#### Test 2: Check User Profile Endpoint

Navigate to: https://localhost:7148/profile

Or test via Swagger:
1. Go to https://localhost:5000
2. Click on `/api/users/me` GET endpoint
3. Click "Try it out"
4. Click "Execute"

**Expected Response**:
```json
{
  "id": "...",
  "firstName": "Customer",
  "lastName": "User",
  "email": "customer@carrental.com",
  "phoneNumber": "",
  "driverLicenseNumber": "",
  "dateOfBirth": null,
  "address": null,
  "registrationDate": "2024-12-...",
  "tier": 0
}
```

#### Test 3: Try Creating a Rental

1. Navigate to https://localhost:7148/vehicles/browse
2. Click "Rent Now" on any vehicle
3. Fill in the booking form
4. Click "Book Vehicle"

**Expected**: Rental should be created successfully without 404 errors

## Verification Checklist

- [ ] Database script executed successfully
- [ ] All AspNetUsers columns exist
- [ ] Backend restarted successfully
- [ ] Browser cache cleared
- [ ] Logged out and back in with new token
- [ ] `/api/users/me` returns 200 OK (not 404)
- [ ] Profile page loads without errors
- [ ] Can view rental history
- [ ] Can create new rentals

## Troubleshooting

### Still Getting 404 on /api/users/me

**Check JWT Token Claims**:
```
1. Login
2. Open DevTools (F12)
3. Go to Application > Local Storage
4. Copy the authToken value
5. Go to https://jwt.io
6. Paste the token
7. Check the payload section
```

**Expected Claims**:
```json
{
  "nameid": "user-id-here",
  "unique_name": "email@example.com",  // This should be email, not username
  "email": "email@example.com",
  "username": "customer",
  "role": "Customer"
}
```

If `unique_name` is showing a username instead of email, the backend wasn't restarted properly.

### Still Getting DriverLicenseNumber Error

**Check if column exists**:
```sql
SELECT * FROM sys.columns 
WHERE object_id = OBJECT_ID('AspNetUsers') 
AND name = 'DriverLicenseNumber';
```

**If returns no rows**: Run the fix script again

**If returns a row**: Check if there's a cached migration issue:
```powershell
cd Backend
dotnet ef database drop
dotnet ef database update
```

### Cannot Create Rentals

**Check if UserId is being passed correctly**:
1. Open browser DevTools
2. Go to Network tab
3. Create a rental
4. Look for the POST request to `/api/rentals`
5. Check the payload

**Expected Payload**:
```json
{
  "vehicleId": 1,
  "startDate": "2024-12-10",
  "endDate": "2024-12-15",
  "pricingStrategy": "standard"
}
```

Note: `userId` should NOT be in the payload for customers - it's extracted from the JWT token.

## Additional Diagnostic Scripts

### Check User and Role Assignment
```sql
SELECT 
    u.UserName,
    u.Email,
    r.Name as RoleName,
    u.EmailConfirmed,
    u.DriverLicenseNumber
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
ORDER BY r.Name, u.Email;
```

### Check Rentals Foreign Key
```sql
SELECT 
    fk.name as ConstraintName,
    OBJECT_NAME(fk.parent_object_id) as TableName,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) as ColumnName,
    OBJECT_NAME(fk.referenced_object_id) as ReferencedTable
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fc ON fk.object_id = fc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'Rentals';
```

**Expected**: Should show FK_Rentals_AspNetUsers_UserId

## What Changed

### Before:
- JWT token used username for `ClaimTypes.Name`
- `User.Identity.Name` returned username
- UserManager lookup by username failed (because we lookup by email)
- 404 error on `/api/users/me`

### After:
- JWT token uses email for `ClaimTypes.Name`
- `User.Identity.Name` returns email
- UserManager lookup by email succeeds
- User profile loads correctly
- Rentals can be created

## Files Modified

1. ? `Backend/Application/Services/JwtService.cs` - Fixed JWT claims
2. ? `Backend/Controllers/UsersController.cs` - Improved authentication
3. ? `Backend/fix_aspnetusers_columns.sql` - Database fix script (NEW)
4. ? `Backend/diagnose_user_issue.sql` - Diagnostic script (NEW)
5. ? `FIX_USERS_ME_404_ERROR.md` - This documentation (NEW)

## Next Steps After Fix

Once the fix is applied and verified:

1. **Test Customer Rental Flow**:
   - Browse vehicles
   - Create rental
   - View rental history
   - Report damage (if in active rental)

2. **Test Admin Features**:
   - View all customers
   - Create rentals for customers
   - View reports

3. **Monitor Logs**:
   ```powershell
   # Check backend logs for any remaining errors
   cd Backend
   dotnet run | Tee-Object -FilePath backend.log
   ```

4. **Update Documentation**:
   - Mark this fix as complete in your project docs
   - Update QUICK_START.md if needed

## Prevention

To avoid this issue in future migrations:

1. **Always run both EF migrations AND SQL scripts**
2. **Verify column existence before using them**
3. **Test JWT token claims after auth changes**
4. **Clear browser cache when backend auth changes**
5. **Use diagnostic scripts to verify database state**

## Support

If you continue to have issues:

1. Run diagnostic script: `Backend/diagnose_user_issue.sql`
2. Check backend logs for specific error messages
3. Verify all steps in this guide were completed
4. Check that old Customers table has been removed
5. Ensure all users have the correct role assignments
