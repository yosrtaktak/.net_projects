# ?? Database Migration Execution Log

## Migration: RemoveCustomersTable
**Date**: December 2024
**Purpose**: Migrate from Customers table to ApplicationUser (AspNetUsers) table

---

## ? Pre-Migration Checklist

- [x] Backend builds successfully
- [x] Frontend builds successfully  
- [x] Migration file reviewed and verified
- [x] Backup script created
- [ ] **PENDING**: Database backup created
- [ ] **PENDING**: Migration applied

---

## ?? Migration Steps

### Step 1: Backup Database (CRITICAL!)

**?? IMPORTANT: Run this SQL script BEFORE applying the migration!**

Script location: `Backend/backup_database_before_migration.sql`

```sql
-- This will create backup tables:
-- - Customers_Backup_BeforeMigration
-- - Rentals_With_Customer_Backup
```

### Step 2: Apply Migration

```bash
cd Backend
dotnet ef database update
```

### Step 3: Verify Migration

```sql
-- Check that:
-- 1. Customers table is removed
-- 2. AspNetUsers has new columns
-- 3. Rentals.UserId is populated
-- 4. No orphaned rentals
```

---

## ?? Migration Details

### What the Migration Does:

1. **Adds columns to AspNetUsers**:
   - DriverLicenseNumber (nvarchar(50))
   - DateOfBirth (datetime2)
   - Address (nvarchar(500))
   - RegistrationDate (datetime2)
   - Tier (int)

2. **Migrates customer data**:
   - Copies data from Customers to AspNetUsers (matched by email)
   - Preserves all customer information

3. **Updates Rentals table**:
   - Adds UserId column (nvarchar(450))
   - Populates UserId from CustomerId via email lookup
   - Removes CustomerId column
   - Creates foreign key to AspNetUsers

4. **Cleanup**:
   - Drops Customers table
   - Creates index on DriverLicenseNumber for uniqueness

---

## ?? Potential Issues and Solutions

### Issue 1: Customers without matching AspNetUsers
**Cause**: Customer record exists but no user account
**Solution**: Migration will skip these records. Review and create user accounts manually if needed.

### Issue 2: Multiple customers with same email
**Cause**: Data inconsistency
**Solution**: Review and merge duplicate records before migration.

### Issue 3: Orphaned rentals
**Cause**: Rentals with invalid CustomerId
**Solution**: Migration will fail. Fix data inconsistencies first.

---

## ?? Rollback Procedure

If migration fails or causes issues:

```bash
cd Backend
dotnet ef database update InitialCreate
```

This will:
- Recreate Customers table
- Restore CustomerId in Rentals
- Copy data back from AspNetUsers
- Remove customer columns from AspNetUsers

---

## ? Post-Migration Verification

After migration, verify:

1. **AspNetUsers table**:
   ```sql
   SELECT TOP 10 
       Email, FirstName, LastName, 
       DriverLicenseNumber, Tier
   FROM AspNetUsers 
   WHERE DriverLicenseNumber IS NOT NULL;
   ```

2. **Rentals table**:
   ```sql
   SELECT TOP 10 
       r.Id, r.UserId, 
       u.Email, u.FirstName, u.LastName
   FROM Rentals r
   JOIN AspNetUsers u ON r.UserId = u.Id;
   ```

3. **Customers table (should not exist)**:
   ```sql
   -- This should fail with "Invalid object name 'Customers'"
   SELECT * FROM Customers;
   ```

4. **Test application**:
   - Login as customer
   - View profile
   - Book a vehicle
   - View rental history

---

## ?? Notes

- Migration preserves all data
- Rollback is available if needed
- Foreign key constraints maintain referential integrity
- Indexes are created for performance

---

## Status: READY TO EXECUTE

**Next Action**: 
1. ?? Run backup script first!
2. Apply migration: `dotnet ef database update`
3. Verify results
4. Test application

