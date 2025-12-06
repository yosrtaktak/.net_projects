# Quick Fix Checklist - Customer Booking Issues

## ?? Current Issues
1. ? Customer gets **404 error** on `/api/customers/me`
2. ? Customer cannot cancel rentals (blocked by authorization)
3. ? Booking page shows: "Unable to load your customer profile"

## ? Solutions Applied

### 1. Code Fix - RentalsController.cs
**Status**: ? **FIXED** (requires backend restart)

**Change Made:**
Removed `[Authorize(Roles = "Admin,Employee")]` from the cancel endpoint to allow customers to cancel their own Reserved rentals.

**File**: `Backend/Controllers/RentalsController.cs`
**Line**: Cancel endpoint
**Action Required**: **Restart backend server**

### 2. Database Fix - Missing Customer Records
**Status**: ? **NEEDS TO BE RUN**

**SQL Script**: `Backend/create_missing_customer_records.sql`

**Quick Run Command:**
```sql
USE CarRentalDB;
GO

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
GO
```

## ?? Action Steps

### Step 1: Restart Backend (To Apply Code Fix)
```powershell
# Stop the current backend (Ctrl+C in backend terminal)
# Then restart:
cd Backend
dotnet run
```

**Expected Output:**
```
Now listening on: https://localhost:5000
Application started. Press Ctrl+C to shut down.
```

### Step 2: Run SQL Script (To Fix 404 Error)

**Option A: Using SQL Server Management Studio (SSMS)**
1. Open SSMS
2. Connect to your SQL Server
3. Open file: `Backend/create_missing_customer_records.sql`
4. Execute (F5)

**Option B: Using Azure Data Studio**
1. Open Azure Data Studio
2. Connect to your SQL Server
3. New Query
4. Paste the script above
5. Run

**Option C: Using Command Line**
```powershell
sqlcmd -S localhost -d CarRentalDB -E -i Backend/create_missing_customer_records.sql
```

### Step 3: Verify Fixes

**Test 1: Check Customer Record Created**
```sql
-- Replace with your actual customer email
SELECT * FROM Customers WHERE Email = 'customer@example.com';
```

**Expected**: One row returned with customer details

**Test 2: Check API Endpoint**
```powershell
# Get your JWT token from browser (F12 -> Application -> Local Storage -> authToken)
$token = "YOUR_JWT_TOKEN_HERE"
curl -H "Authorization: Bearer $token" https://localhost:5000/api/customers/me
```

**Expected**: Customer profile JSON (not 404)

**Test 3: Test in Browser**
1. Navigate to `https://localhost:7148/book-vehicle`
2. Open browser console (F12)
3. Check Network tab for `/api/customers/me`

**Expected**: 200 OK (not 404)

## ?? Quick Verification

### ? Success Indicators
- [ ] Backend restarted successfully
- [ ] SQL script executed without errors
- [ ] Customer record exists in database
- [ ] `/api/customers/me` returns 200 OK
- [ ] Booking page loads without errors
- [ ] Customer info displays on booking page
- [ ] Can cancel Reserved rental

### ? Still Having Issues?

**If still getting 404:**
1. Verify customer email matches in both `AspNetUsers` and `Customers` tables
2. Check user has "Customer" role
3. Clear browser cache
4. Logout and login again

**If cannot cancel rental:**
1. Check rental status is "Reserved" (not Active or Completed)
2. Verify you own the rental
3. Check browser console for errors
4. Verify backend restarted

## ?? Quick Diagnostic Queries

### Check User and Customer Link
```sql
SELECT 
    u.Email as UserEmail,
    r.Name as Role,
    c.Id as CustomerId,
    c.FirstName,
    c.LastName,
    CASE 
        WHEN c.Id IS NULL THEN '? No Customer Record'
        ELSE '? Customer Record Exists'
    END as Status
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
LEFT JOIN Customers c ON u.Email = c.Email
WHERE r.Name = 'Customer'
ORDER BY u.Email;
```

### Check Rental Status for Cancellation
```sql
SELECT 
    r.Id,
    r.Status,
    r.StartDate,
    r.EndDate,
    v.Brand + ' ' + v.Model as Vehicle,
    c.Email as CustomerEmail,
    CASE 
        WHEN r.Status = 0 THEN '? Can Cancel (Reserved)'
        WHEN r.Status = 1 THEN '? Cannot Cancel (Active)'
        WHEN r.Status = 2 THEN '? Cannot Cancel (Completed)'
        WHEN r.Status = 3 THEN 'Already Cancelled'
        ELSE 'Unknown Status'
    END as CancellationEligibility
FROM Rentals r
JOIN Customers c ON r.CustomerId = c.Id
JOIN Vehicles v ON r.VehicleId = v.Id
WHERE c.Email = 'customer@example.com'
ORDER BY r.CreatedAt DESC;
```

## ?? Support

### Common Error Messages and Solutions

| Error Message | Cause | Solution |
|---------------|-------|----------|
| "404 Not Found" on /api/customers/me | No Customer record | Run SQL script (Step 2) |
| "Unable to load customer profile" | Same as above | Run SQL script (Step 2) |
| "403 Forbidden" on cancel | Not your rental or wrong role | Check ownership and role |
| "Only reserved rentals can be cancelled" | Rental status not Reserved | Can only cancel Reserved rentals |
| Backend not responding | Backend not running | Restart backend (Step 1) |

### Still Need Help?

1. **Check backend logs** for exceptions
2. **Check browser console** (F12) for errors
3. **Verify database connection** is working
4. **Check JWT token** is valid (not expired)
5. **Review** `FIX_CUSTOMER_CANCELLATION_AUTHORIZATION.md` for detailed info

## ?? Success!

Once both steps are complete:
- ? Customer can access booking page
- ? Customer profile loads correctly
- ? Customer can book vehicles
- ? Customer can cancel Reserved rentals
- ? Security is maintained (ownership verification)

---

**Last Updated**: December 2024
**Priority**: ?? **HIGH** - Blocking customer features
**Impact**: Customer booking and cancellation
**Estimated Time**: 5-10 minutes
