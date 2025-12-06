# ?? Final Migration Steps - Customer to ApplicationUser

## Current Status: Ready to Migrate

? **Frontend**: Build successful (all fixes applied)
? **Backend**: Build successful (all fixes applied)  
?? **Database**: Needs migration

---

## Migration Approach: Direct SQL Script

Since EF Migrations had issues with partial migration state, we're using a **direct SQL script** approach which is:
- ? More reliable
- ? Easier to troubleshoot
- ? Includes full transaction rollback on error
- ? Provides detailed progress output

---

## ?? Step-by-Step Migration Guide

### Step 1: Backup Database (CRITICAL!)

**Option A - SQL Server Management Studio:**
1. Right-click on `CarRentalDB`
2. Tasks ? Back Up...
3. Save to a safe location
4. Note the backup file path

**Option B - SQL Command:**
```sql
BACKUP DATABASE CarRentalDB 
TO DISK = 'C:\Backups\CarRentalDB_BeforeMigration.bak'
WITH FORMAT, INIT, NAME = 'Before Customer Migration';
```

### Step 2: Check Current Database State (Optional)

Run this script to see what's currently in the database:
```bash
# File: Backend/check_database_state.sql
```

### Step 3: Clean Up Any Partial Migration (If Needed)

If you had any previous failed migration attempts:
```bash
# File: Backend/cleanup_partial_migration.sql
```

### Step 4: Run the Main Migration Script

**This is the main migration!**

1. Open SQL Server Management Studio or Azure Data Studio
2. Connect to your database server
3. Open file: `Backend/migrate_to_applicationuser.sql`
4. Review the script (it has comments explaining each step)
5. **Execute the script**

The script will:
- ? Add customer columns to AspNetUsers
- ? Migrate all customer data
- ? Add UserId column to Rentals
- ? Populate UserId from CustomerId
- ? Drop old constraints and columns
- ? Create new foreign keys and indexes
- ? Drop Customers table
- ? Update migration history
- ? Verify the migration
- ? Commit transaction (or rollback on error)

**Expected output:**
```
========================================
CUSTOMER TO APPLICATIONUSER MIGRATION
========================================

Starting at: 2024-12-02 18:00:00

1. Adding customer columns to AspNetUsers...
   ? Added DriverLicenseNumber
   ? Added DateOfBirth
   ? Added Address
   ? Added RegistrationDate
   ? Added Tier

2. Migrating customer data to AspNetUsers...
   ? Migrated 5 customer records

3. Adding UserId column to Rentals...
   ? Added UserId column

4. Populating UserId in Rentals from CustomerId...
   ? Populated UserId for 10 rentals

5. Making UserId column NOT NULL...
   ? Changed UserId to NOT NULL

6. Removing old CustomerId constraints...
   ? Dropped FK_Rentals_Customers_CustomerId
   ? Dropped IX_Rentals_CustomerId

7. Dropping CustomerId column...
   ? Dropped CustomerId column

8. Creating new indexes and foreign key...
   ? Created IX_Rentals_UserId
   ? Created IX_AspNetUsers_DriverLicenseNumber
   ? Created FK_Rentals_AspNetUsers_UserId

9. Dropping Customers table...
   ? Dropped Customers table

10. Updating migration history...
   ? Added migration record

========================================
VERIFICATION
========================================
Users with customer data: 5
Total rentals: 10
Customers table: DROPPED ?

========================================
MIGRATION COMPLETED SUCCESSFULLY!
========================================
Completed at: 2024-12-02 18:00:15

Transaction COMMITTED
```

### Step 5: Verify Migration

Run these verification queries:

```sql
-- 1. Check AspNetUsers has customer data
SELECT TOP 5 
    Email, FirstName, LastName, 
    DriverLicenseNumber, Tier, RegistrationDate
FROM AspNetUsers 
WHERE DriverLicenseNumber IS NOT NULL;

-- 2. Check Rentals use UserId
SELECT TOP 5 
    r.Id, r.UserId, 
    u.Email, u.FirstName, u.LastName
FROM Rentals r
JOIN AspNetUsers u ON r.UserId = u.Id;

-- 3. Verify Customers table is gone (should error)
SELECT * FROM Customers; -- Should fail with "Invalid object name"

-- 4. Check migration history
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId;
```

### Step 6: Test the Application

1. **Start Backend:**
   ```bash
   cd Backend
   dotnet run
   ```

2. **Start Frontend:**
   ```bash
   cd Frontend
   dotnet run
   ```

3. **Test Key Features:**
   - ? Login as customer (customer@carrental.com / Customer@123)
   - ? View profile
   - ? Browse vehicles
   - ? Book a vehicle
   - ? View rental history
   - ? Login as admin/employee
   - ? View customer list
   - ? Create rental for customer
   - ? View reports

---

## ?? Rollback Plan (If Something Goes Wrong)

If the migration fails or causes issues:

### Option 1: Restore from Backup
```sql
-- Close all connections to database first
USE master;
GO

ALTER DATABASE CarRentalDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

RESTORE DATABASE CarRentalDB 
FROM DISK = 'C:\Backups\CarRentalDB_BeforeMigration.bak'
WITH REPLACE;
GO

ALTER DATABASE CarRentalDB SET MULTI_USER;
GO
```

### Option 2: Manual Rollback (If backup not available)
The migration script uses a transaction, so if it fails, it automatically rolls back. The database will be unchanged.

---

## ?? What This Migration Does

### Before Migration:
```
Tables:
- Customers (Id, Email, FirstName, LastName, etc.)
- AspNetUsers (Id, Email, UserName, etc.)
- Rentals (Id, CustomerId ? Customers.Id, VehicleId, etc.)

Data duplication:
- Customer info in both Customers AND AspNetUsers
- Rentals linked to Customers table
```

### After Migration:
```
Tables:
- AspNetUsers (Id, Email, UserName, DriverLicenseNumber, Tier, etc.)
- Rentals (Id, UserId ? AspNetUsers.Id, VehicleId, etc.)
- ? Customers table REMOVED

Single source of truth:
- All customer info in AspNetUsers only
- Rentals directly linked to AspNetUsers
- No data duplication
```

---

## ? Post-Migration Checklist

After successful migration:

- [ ] Backup completed
- [ ] Migration script executed successfully
- [ ] Verification queries passed
- [ ] Backend starts without errors
- [ ] Frontend starts without errors
- [ ] Can login as customer
- [ ] Can view profile
- [ ] Can book vehicle
- [ ] Can view rentals
- [ ] Admin can view customer list
- [ ] Admin can create rentals

---

## ?? Migration Files Reference

| File | Purpose |
|------|---------|
| `Backend/migrate_to_applicationuser.sql` | **Main migration script** - Run this! |
| `Backend/check_database_state.sql` | Check current database state |
| `Backend/cleanup_partial_migration.sql` | Clean up failed migration attempts |
| `Backend/backup_database_before_migration.sql` | Create backup tables |

---

## ?? Troubleshooting

### Issue: "Foreign key constraint failed"
**Cause**: Orphaned rentals without matching users
**Solution**: The script automatically deletes orphaned rentals

### Issue: "Column already exists"
**Cause**: Partial migration from previous attempt
**Solution**: Run `cleanup_partial_migration.sql` first

### Issue: "Transaction rolled back"
**Cause**: Error during migration
**Solution**: Check error message, database is unchanged, fix issue and retry

### Issue: Cannot find Customers table after migration
**Cause**: Migration successful!
**Solution**: This is expected - use AspNetUsers now

---

## ?? Success Indicators

You'll know the migration was successful when:

1. ? Script shows "MIGRATION COMPLETED SUCCESSFULLY!"
2. ? Verification shows "Customers table: DROPPED ?"
3. ? Application starts without errors
4. ? Can login and use all features
5. ? No 404 errors when accessing /api/users/me

---

## ?? Need Help?

If you encounter any issues:
1. Check the error message in the script output
2. Review the rollback section above
3. The transaction ensures database safety - if migration fails, nothing changes
4. You can safely re-run the script after fixing issues

---

**Status**: ?? **READY TO MIGRATE**

**Next Action**: Run `Backend/migrate_to_applicationuser.sql` in SQL Server Management Studio

**Estimated Time**: 5-10 minutes (mostly verification and testing)

**Risk Level**: Low (full transaction with automatic rollback)

