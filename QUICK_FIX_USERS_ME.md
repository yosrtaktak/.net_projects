# QUICK FIX: 404 Error on /api/users/me

## The Problem
Customer users getting **404 (Not Found)** on `/api/users/me` endpoint, preventing rental creation.

## The Solution (3 Steps)

### 1?? Run Database Fix Script

Open a new PowerShell window and run:

```powershell
# Navigate to project directory
cd C:\Users\ugran\source\repos\yosrtaktak\.net_projects\Backend

# Run the fix script (choose one method)

# Method A: Using sqlcmd
sqlcmd -S localhost -d CarRentalDB -E -i fix_aspnetusers_columns.sql

# Method B: Using SQL Server Management Studio
# 1. Open SSMS
# 2. Connect to your server
# 3. File > Open > File > Select fix_aspnetusers_columns.sql
# 4. Press F5 to execute
```

**Expected Output**:
```
? Added FirstName column (or already exists)
? Added LastName column (or already exists)
? Added DriverLicenseNumber column (or already exists)
? Added DateOfBirth column (or already exists)
? Added Address column (or already exists)
? Added RegistrationDate column (or already exists)
? Added Tier column (or already exists)
? Rentals table has UserId column
? Foreign key FK_Rentals_AspNetUsers_UserId exists
=== FIX COMPLETE ===
```

### 2?? Restart Backend

**Stop the current backend** (press Ctrl+C in the backend terminal)

Then restart:

```powershell
cd C:\Users\ugran\source\repos\yosrtaktak\.net_projects\Backend
dotnet run
```

**Wait for**:
```
Now listening on: https://localhost:5000
Now listening on: http://localhost:5002
Application started. Press Ctrl+C to shut down.
```

### 3?? Clear Browser & Re-login

**CRITICAL**: You MUST get a new JWT token!

1. **Open your browser** (where the frontend is open)
2. **Press F12** to open DevTools
3. **Go to Application tab** ? Local Storage ? https://localhost:7148
4. **Delete the authToken** entry (or click "Clear All")
5. **Close DevTools**
6. **Click Logout** in your app
7. **Login again** with your customer credentials
   - Username: `customer`
   - Password: `Customer@123`

### 4?? Test It Works

1. Navigate to: https://localhost:7148/vehicles/browse
2. Click **"Rent Now"** on any vehicle
3. Fill in the booking form
4. Click **"Book Vehicle"**

**Expected**: ? Success! Rental created without 404 errors

---

## Verify Backend Is Running

Check these URLs in your browser:

- Backend API: https://localhost:5000 (should show Swagger UI)
- User Profile Test: 
  1. Login first at https://localhost:7148/login
  2. Then try https://localhost:7148/profile

---

## What Was Fixed?

### Problem 1: JWT Token Claims
- **Before**: Token used username for `ClaimTypes.Name`
- **After**: Token uses email for `ClaimTypes.Name`
- **Why**: `User.Identity.Name` needs to return email for user lookup

### Problem 2: Missing Database Columns
- **Before**: AspNetUsers missing customer-specific columns
- **After**: All required columns added (DriverLicenseNumber, DateOfBirth, etc.)
- **Why**: ApplicationUser entity expects these columns

### Problem 3: Authentication Fallback
- **Before**: Only checked `User.Identity.Name`
- **After**: Checks multiple claim types for email
- **Why**: Handles different token formats gracefully

---

## Troubleshooting

### ? Still Getting 404?

**Check if backend restarted properly:**
```powershell
# Look for the backend process
Get-Process | Where-Object {$_.ProcessName -like "*dotnet*"}
```

**Check if you got a new token:**
1. F12 ? Application ? Local Storage
2. Look at authToken value
3. Copy it to https://jwt.io
4. Check the payload - `unique_name` should show email@example.com, not a username

### ? Database Script Failed?

**Check SQL Server is running:**
```powershell
Get-Service | Where-Object {$_.Name -like "*SQL*"}
```

**Try connecting manually:**
```powershell
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

### ? Can't Login?

**Use test accounts:**
- Admin: `admin@carrental.com` / `Admin@123`
- Employee: `employee@carrental.com` / `Employee@123`  
- Customer: `customer@carrental.com` / `Customer@123`

Or register a new account:
1. Go to https://localhost:7148/register
2. Create new customer account
3. Login with new credentials

---

## Files Changed

? `Backend/Application/Services/JwtService.cs` - Fixed JWT claims  
? `Backend/Controllers/UsersController.cs` - Improved auth handling  
? `Backend/fix_aspnetusers_columns.sql` - Database fix script (**NEW**)  
? `Backend/diagnose_user_issue.sql` - Diagnostic queries (**NEW**)  
? `FIX_USERS_ME_404_ERROR.md` - Detailed documentation (**NEW**)  
? `QUICK_FIX_USERS_ME.md` - This guide (**NEW**)

---

## Need More Help?

1. Run diagnostic: Open `Backend/diagnose_user_issue.sql` in SSMS and execute
2. Check full documentation: See `FIX_USERS_ME_404_ERROR.md`
3. Check backend console for error messages
4. Check browser console (F12) for frontend errors

---

## Summary

The fix involves:
1. ??? **Database**: Adding missing columns to AspNetUsers
2. ?? **Backend**: Fixing JWT token to use email in Name claim
3. ?? **Browser**: Getting a new token by logging out/in

**Total time**: ~2-3 minutes

Once complete, customer rental creation should work perfectly! ??
