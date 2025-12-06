# ?? Fix Application Checklist - Customer Rental 404 Error

## ? Quick Overview
- **Problem**: Customer users getting 404 on `/api/users/me`, blocking rental creation
- **Root Cause**: JWT token using username instead of email in Name claim
- **Fix Time**: 2-3 minutes
- **Files Changed**: 2 backend files (already modified)
- **Action Required**: Run database script + restart backend + re-login

---

## ? Pre-Flight Checks

- [ ] Backend is currently running (https://localhost:5000)
- [ ] Frontend is currently running (https://localhost:7148)
- [ ] SQL Server is running
- [ ] Can connect to database (CarRentalDB)
- [ ] Have customer login credentials ready

**Test Credentials**:
- Username: `customer` or `customer@carrental.com`
- Password: `Customer@123`

---

## ?? Fix Application Steps

### ?? Step 1: Run Database Fix Script (30 seconds)

Open **new PowerShell window**:

```powershell
cd C:\Users\ugran\source\repos\yosrtaktak\.net_projects\Backend
sqlcmd -S localhost -d CarRentalDB -E -i fix_aspnetusers_columns.sql
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

**Verification**:
- [ ] Script executed successfully
- [ ] No error messages
- [ ] All columns show as added or existing

**If Failed**: 
- Check SQL Server is running: `Get-Service MSSQLSERVER`
- Try SSMS: Open `fix_aspnetusers_columns.sql` and execute

---

### ?? Step 2: Stop Backend (5 seconds)

In the **backend terminal**:
- Press `Ctrl + C`
- Wait for "Application is shutting down..."

**Verification**:
- [ ] Backend process stopped
- [ ] No more log messages appearing

---

### ?? Step 3: Restart Backend (30 seconds)

In the **same terminal**:

```powershell
cd C:\Users\ugran\source\repos\yosrtaktak\.net_projects\Backend
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

**Verification**:
- [ ] "Application started" message appears
- [ ] No red error messages
- [ ] Swagger UI loads at https://localhost:5000

**If Failed**:
- Run `dotnet clean` then `dotnet build`
- Check for compilation errors
- Ensure ports 5000/5002 are free

---

### ?? Step 4: Clear Browser Cache (1 minute)

**CRITICAL STEP** - Old JWT tokens won't work!

In your **browser** where frontend is open:

1. Press `F12` to open DevTools
2. Go to **Application** tab (Chrome) or **Storage** tab (Firefox)
3. Expand **Local Storage** in left panel
4. Click on **https://localhost:7148**
5. Right-click on **authToken** ? Delete
6. Or click **Clear All** button at top
7. Close DevTools (`F12` again)

**Verification**:
- [ ] authToken removed from Local Storage
- [ ] Local Storage is empty or authToken is gone

---

### ?? Step 5: Logout (if logged in)

In the **frontend app**:

1. Click your username in top-right corner (if visible)
2. Click **Logout** button
3. You should be redirected to login page

**Or** just navigate to: https://localhost:7148/login

**Verification**:
- [ ] On login page
- [ ] Not logged in (no username showing)

---

### ?? Step 6: Login with Customer Account (30 seconds)

On the **login page**:

1. Username: `customer@carrental.com` (or just `customer`)
2. Password: `Customer@123`
3. Click **Login**

**Expected**:
- ? Login successful
- ? Redirected to dashboard or home
- ? Username shows in top-right

**Verification**:
- [ ] Login succeeded
- [ ] No error messages
- [ ] Can see navigation menu

---

### ?? Step 7: Verify Profile Loads (15 seconds)

Navigate to: https://localhost:7148/profile

**Expected**:
- ? Profile page loads
- ? User info displayed
- ? No 404 errors in console (F12 ? Console tab)

**Verification**:
- [ ] Profile page visible
- [ ] Email, name fields populated
- [ ] No errors in browser console

---

### ?? Step 8: Test Rental Creation (30 seconds)

1. Navigate to: https://localhost:7148/vehicles/browse
2. Click **Rent Now** on any available vehicle (green status)
3. Select start date (today or future)
4. Select end date (after start date)
5. Choose pricing strategy (Standard is fine)
6. Click **Book Vehicle**

**Expected**:
- ? Success message appears
- ? Redirected to rental history
- ? New rental appears in list
- ? No 404 errors in console

**Verification**:
- [ ] Rental created successfully
- [ ] Success notification shown
- [ ] Rental appears in history
- [ ] No console errors

---

## ?? Final Verification

### Check JWT Token (Optional but Recommended)

1. F12 ? Application ? Local Storage
2. Copy **authToken** value
3. Go to https://jwt.io
4. Paste token in "Encoded" box
5. Look at "Payload" section (right side, middle box)

**Expected Claims**:
```json
{
  "nameid": "some-guid-here",
  "unique_name": "customer@carrental.com",  // ? MUST be email
  "email": "customer@carrental.com",
  "username": "customer",
  "role": "Customer",
  "exp": 1234567890
}
```

**Key Check**:
- [ ] `unique_name` is an **email address** (not just "customer")
- [ ] `email` claim exists
- [ ] `role` is "Customer"

**If unique_name is NOT an email**:
- Backend didn't restart properly
- Go back to Step 2 and restart again
- Clear cache and re-login

---

### Test All Features

Quick smoke test of customer features:

- [ ] **Profile**: Can view at `/profile`
- [ ] **Browse Vehicles**: Can see vehicles at `/vehicles/browse`
- [ ] **Create Rental**: Can book a vehicle
- [ ] **View Rentals**: Can see history at `/my-rentals`
- [ ] **Cancel Rental**: Can cancel a Reserved rental (if any)

---

## ? Success Criteria

**You're done when ALL of these are true**:

1. ? Database script ran successfully
2. ? Backend restarted without errors
3. ? Browser cache cleared
4. ? Logged out and back in
5. ? Profile page loads (no 404)
6. ? Can create rentals
7. ? JWT token has email in unique_name
8. ? No console errors

---

## ? Troubleshooting Quick Fixes

### Problem: Script fails with "Cannot open database"
**Solution**:
```powershell
# Check SQL Server is running
Get-Service MSSQLSERVER
# If stopped, start it
Start-Service MSSQLSERVER
```

### Problem: Backend fails to start
**Solution**:
```powershell
cd Backend
dotnet clean
dotnet build
# Check for errors, fix them, then:
dotnet run
```

### Problem: Still getting 404 after all steps
**Solution**:
1. Close browser completely
2. Reopen browser
3. Go to https://localhost:7148/login
4. Login again
5. Try again

### Problem: "Port 5000 already in use"
**Solution**:
```powershell
# Kill all dotnet processes
Get-Process | Where-Object {$_.ProcessName -eq "dotnet"} | Stop-Process -Force
# Start backend again
cd Backend
dotnet run
```

### Problem: Cannot login
**Solution**:
- Try `customer@carrental.com` instead of `customer`
- Check caps lock is off
- Password is: `Customer@123` (exact case)
- Or register new account at `/register`

---

## ?? Help Resources

If you get stuck:

1. **Full Documentation**: Open `FIX_USERS_ME_404_ERROR.md`
2. **Implementation Guide**: Open `FIX_IMPLEMENTATION_SUMMARY.md`
3. **Database Diagnostic**: Run `Backend/diagnose_user_issue.sql` in SSMS
4. **Backend Logs**: Look at terminal where backend is running
5. **Browser Console**: F12 ? Console tab for frontend errors

---

## ?? Completion

Once all checkboxes are ticked:

- [ ] **Mark issue as resolved** in your project tracker
- [ ] **Commit changes to git**:
  ```bash
  git add .
  git commit -m "Fix: Resolve customer 404 error on user profile endpoint"
  git push
  ```
- [ ] **Update project status**: Customer self-service rental working ?
- [ ] **Test with real user flow**: Have someone else try it

---

## ?? Time Tracking

| Step | Expected Time | Actual Time |
|------|--------------|-------------|
| 1. Database Script | 30s | ____ |
| 2. Stop Backend | 5s | ____ |
| 3. Restart Backend | 30s | ____ |
| 4. Clear Cache | 1m | ____ |
| 5. Logout | 5s | ____ |
| 6. Login | 30s | ____ |
| 7. Verify Profile | 15s | ____ |
| 8. Test Rental | 30s | ____ |
| **Total** | **~3 minutes** | **____** |

---

**Status**: Ready to Execute  
**Priority**: High (Blocking customer rentals)  
**Difficulty**: Easy  
**Risk**: Low (Safe database changes, code already tested)

---

## ?? Status Indicators

**Before Starting**: ?? Customer rentals broken  
**After Database Script**: ?? Database ready  
**After Backend Restart**: ?? Backend updated  
**After Re-login**: ?? **Everything Working!**

---

**Good luck! The fix should take just 2-3 minutes to apply.** ??
