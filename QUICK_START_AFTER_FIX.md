# Quick Start After Migration Fix

## ? What Was Fixed

The partial migration issue has been resolved. The database now has all required columns in the `AspNetUsers` table:
- ? Address
- ? DateOfBirth
- ? DriverLicenseNumber
- ? RegistrationDate
- ? Tier

## ?? Start the Application

### 1. Start Backend
```powershell
cd Backend
dotnet run
```

**Expected output:**
```
Now listening on: https://localhost:5000
Now listening on: http://localhost:5002
Application started. Press Ctrl+C to shut down.
```

### 2. Verify Backend is Running
Open browser to: `https://localhost:5000/swagger`

### 3. Test Authentication

#### Login as Admin
```
POST https://localhost:5000/api/auth/login
{
  "username": "admin@example.com",
  "password": "Admin123!"
}
```

#### Login as Customer
```
POST https://localhost:5000/api/auth/login
{
  "username": "customer@example.com",
  "password": "Customer123!"
}
```

### 4. Test User Profile
```
GET https://localhost:5000/api/users/me
Authorization: Bearer <your-token-here>
```

**Expected response:**
```json
{
  "id": "...",
  "email": "admin@example.com",
  "firstName": "Admin",
  "lastName": "User",
  "phoneNumber": null,
  "address": null,
  "dateOfBirth": null,
  "driverLicenseNumber": null,
  "registrationDate": "0001-01-01T00:00:00",
  "tier": 0,
  "roles": ["Admin"]
}
```

### 5. Start Frontend (Optional)
```powershell
cd Frontend
dotnet run
```

**Expected output:**
```
Now listening on: https://localhost:5001
```

Open browser to: `https://localhost:5001`

## ? Success Criteria

- [ ] Backend starts without errors
- [ ] No "Invalid column name" errors in console
- [ ] Login works (returns JWT token)
- [ ] User profile endpoint returns data with customer fields
- [ ] Swagger UI loads at https://localhost:5000/swagger
- [ ] Frontend can connect to backend (if testing frontend)

## ?? Troubleshooting

### Backend won't start - Port already in use
```powershell
# Kill existing backend process
Stop-Process -Name "Backend" -Force

# Or kill specific port
$processId = (Get-NetTCPConnection -LocalPort 5000).OwningProcess
Stop-Process -Id $processId -Force
```

### Still seeing "Invalid column name" errors
1. Verify columns were added:
```sql
sqlcmd -S localhost -d CarRentalDB -Q "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME IN ('Address', 'DateOfBirth', 'DriverLicenseNumber', 'RegistrationDate', 'Tier')"
```

2. If columns are missing, run the fix script again:
```powershell
cd Backend
sqlcmd -S localhost -d CarRentalDB -E -i fix_partial_migration.sql
```

3. Clean and rebuild:
```powershell
dotnet clean
dotnet build
dotnet run
```

### Database connection failed
Check connection string in `Backend/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CarRentalDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

## ?? Next Steps

After verifying the backend works:

1. **Test Rentals**
   - Create a rental using the new `UserId` field
   - Verify existing rentals work (if any)

2. **Test Customer Features**
   - Update user profile with customer data
   - Test rental creation by customer
   - Test rental history

3. **Update Existing Data** (if needed)
   - If you have existing users that need customer data populated
   - Run data migration script to populate fields

4. **Frontend Testing**
   - Login page
   - User profile page
   - Rental booking
   - Admin dashboard

## ?? Related Documentation

- `MIGRATION_FIX_SUMMARY.md` - Detailed explanation of what was fixed
- `MIGRATION_COMPLETE_BACKEND_STARTED.md` - Original migration documentation
- `Backend/fix_partial_migration.sql` - SQL script that fixed the issue

---

**Status:** Ready to test! ??

Start with backend testing first, then move to frontend once backend is verified.
