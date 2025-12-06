# Migration Fix Summary

**Date:** December 5, 2024  
**Issue:** Partial migration failure - columns not added to database  
**Status:** ? RESOLVED

---

## Problem

The migration `20251205141715_MigrateToApplicationUser` failed during execution with the error:
```
There is already an object named 'VehicleDamages' in the database.
```

This caused a **partial migration** where:
- ? Migration was marked as applied in `__EFMigrationsHistory`
- ? **Columns were NOT added to the database** (Address, DateOfBirth, DriverLicenseNumber, RegistrationDate, Tier)
- ? Indexes were NOT created
- ? Seed data was NOT inserted

The application then failed with:
```
Invalid column name 'Address'.
Invalid column name 'DateOfBirth'.
Invalid column name 'DriverLicenseNumber'.
Invalid column name 'RegistrationDate'.
Invalid column name 'Tier'.
```

---

## Root Cause

The migration included creating the `VehicleDamages` table, but this table **already existed** in the database from a previous migration. When EF Core tried to execute:

```sql
CREATE TABLE [VehicleDamages] (...)
```

It failed, causing the entire migration transaction to **partially rollback**, leaving the database in an inconsistent state.

---

## Solution

Created and executed `fix_partial_migration.sql` which:

### 1. **Manually Added Missing Columns**
```sql
ALTER TABLE AspNetUsers ADD Address NVARCHAR(500) NULL;
ALTER TABLE AspNetUsers ADD DateOfBirth DATETIME2 NULL;
ALTER TABLE AspNetUsers ADD DriverLicenseNumber NVARCHAR(50) NULL;
ALTER TABLE AspNetUsers ADD RegistrationDate DATETIME2 NOT NULL DEFAULT ('0001-01-01T00:00:00.0000000');
ALTER TABLE AspNetUsers ADD Tier INT NOT NULL DEFAULT 0;
```

### 2. **Created Missing Indexes**
```sql
CREATE UNIQUE NONCLUSTERED INDEX IX_AspNetUsers_DriverLicenseNumber 
ON AspNetUsers(DriverLicenseNumber) 
WHERE DriverLicenseNumber IS NOT NULL;
```

### 3. **Added Missing Seed Data**
- VehicleDamages (3 records)
- Maintenances (4 records)

### 4. **Verified Migration History**
- Migration `20251205141715_MigrateToApplicationUser` was already marked as applied
- No additional history entry needed

---

## Verification

After running the fix script:

```sql
-- All 5 columns now exist in AspNetUsers
Address                 nvarchar     YES     500
DateOfBirth            datetime2    YES     NULL
DriverLicenseNumber    nvarchar     YES     50
RegistrationDate       datetime2    NO      NULL
Tier                   int          NO      NULL
```

**Migration History:**
- ? 20251121193118_InitialCreate
- ? 20251128173930_AddVehicleHistoryData
- ? 20251128174438_AddVehicleDamageTable
- ? 20251205141715_MigrateToApplicationUser

---

## Testing

### Build Status
```bash
cd Backend
dotnet clean
dotnet build
# Result: Build succeeded with 10 warning(s)
```

### Expected Behavior
The backend should now start successfully without errors about missing columns.

### To Test:
1. **Stop any running backend processes:**
   ```powershell
   Stop-Process -Name "Backend" -Force
   ```

2. **Start the backend:**
   ```bash
   cd Backend
   dotnet run
   ```

3. **Verify login works:**
   ```bash
   POST https://localhost:5000/api/auth/login
   {
     "username": "admin@example.com",
     "password": "Admin123!"
   }
   ```

4. **Test user profile (should include customer fields):**
   ```bash
   GET https://localhost:5000/api/users/me
   Authorization: Bearer <token>
   ```

Expected response should include:
```json
{
  "id": "...",
  "email": "...",
  "firstName": "...",
  "lastName": "...",
  "address": null,
  "dateOfBirth": null,
  "driverLicenseNumber": null,
  "registrationDate": "0001-01-01T00:00:00",
  "tier": 0
}
```

---

## Files Created

- `Backend/fix_partial_migration.sql` - Script to complete the partial migration
- `MIGRATION_FIX_SUMMARY.md` - This documentation

---

## Prevention

To prevent similar issues in future migrations:

### 1. **Check for Existing Objects**
Before creating tables/indexes, check if they exist:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TableName')
        BEGIN
            -- Create table
        END
    ");
}
```

### 2. **Use Idempotent Scripts**
For manual SQL migrations, always use IF NOT EXISTS checks:
```sql
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Table') AND name = 'Column')
BEGIN
    ALTER TABLE Table ADD Column TYPE;
END
```

### 3. **Test Migrations on Clean Database**
Before applying migrations to production:
```bash
# Create a test database
sqlcmd -S localhost -Q "CREATE DATABASE CarRentalDB_Test"

# Update connection string to point to test DB
# Run migration
dotnet ef database update

# Verify
sqlcmd -S localhost -d CarRentalDB_Test -Q "SELECT * FROM __EFMigrationsHistory"
```

### 4. **Split Large Migrations**
Instead of one large migration that does multiple things:
- Migration 1: Add columns
- Migration 2: Add indexes
- Migration 3: Seed data
- Migration 4: Remove old tables

This way, if one step fails, you can identify and fix it easily.

---

## Rollback Instructions (If Needed)

If you need to undo this fix for any reason:

```sql
USE CarRentalDB;
GO

BEGIN TRANSACTION;

-- Remove columns
ALTER TABLE AspNetUsers DROP COLUMN DriverLicenseNumber;
ALTER TABLE AspNetUsers DROP COLUMN DateOfBirth;
ALTER TABLE AspNetUsers DROP COLUMN Address;
ALTER TABLE AspNetUsers DROP COLUMN RegistrationDate;
ALTER TABLE AspNetUsers DROP COLUMN Tier;

-- Remove index
DROP INDEX IX_AspNetUsers_DriverLicenseNumber ON AspNetUsers;

-- Remove migration record
DELETE FROM __EFMigrationsHistory 
WHERE MigrationId = '20251205141715_MigrateToApplicationUser';

-- Verify
SELECT * FROM __EFMigrationsHistory;

COMMIT TRANSACTION;
-- Or ROLLBACK TRANSACTION if something goes wrong
```

---

## Next Steps

1. ? Columns added to AspNetUsers
2. ? Indexes created
3. ? Seed data inserted
4. ? Build successful
5. ? Test backend startup
6. ? Test user authentication
7. ? Test rental operations with new UserId field
8. ? Update existing rental records (if any) to use UserId instead of CustomerId

---

## Related Files

- `Backend/Migrations/20251205141715_MigrateToApplicationUser.cs` - Original migration
- `Backend/Core/Entities/ApplicationUser.cs` - User entity with customer properties
- `Backend/Infrastructure/Data/CarRentalDbContext.cs` - DbContext configuration
- `Backend/Controllers/AuthController.cs` - Authentication controller

---

**Status:** Migration fix complete! Database is now consistent with the code model.

?? **Ready to test the application!**
