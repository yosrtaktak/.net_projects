-- Cleanup Script: Reset database to clean state before migration
-- Run this to clean up any partial migration attempts

USE CarRentalDB;
GO

PRINT '===== STARTING CLEANUP =====';
PRINT '';

-- 1. Drop UserId column from Rentals if it exists (from partial migration)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId')
BEGIN
    PRINT 'Removing UserId column from Rentals...';
    
    -- Drop foreign key if exists
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_AspNetUsers_UserId')
    BEGIN
        ALTER TABLE Rentals DROP CONSTRAINT FK_Rentals_AspNetUsers_UserId;
        PRINT '  ? Dropped FK_Rentals_AspNetUsers_UserId';
    END
    
    -- Drop index if exists
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_UserId')
    BEGIN
        DROP INDEX IX_Rentals_UserId ON Rentals;
        PRINT '  ? Dropped IX_Rentals_UserId';
    END
    
    -- Drop column
    ALTER TABLE Rentals DROP COLUMN UserId;
    PRINT '  ? Dropped UserId column';
END
ELSE
BEGIN
    PRINT 'UserId column does not exist in Rentals (OK)';
END

PRINT '';

-- 2. Drop customer-related columns from AspNetUsers if they exist (from partial migration)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
BEGIN
    PRINT 'Removing customer columns from AspNetUsers...';
    
    -- Drop index if exists
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_DriverLicenseNumber')
    BEGIN
        DROP INDEX IX_AspNetUsers_DriverLicenseNumber ON AspNetUsers;
        PRINT '  ? Dropped IX_AspNetUsers_DriverLicenseNumber';
    END
    
    ALTER TABLE AspNetUsers DROP COLUMN DriverLicenseNumber;
    PRINT '  ? Dropped DriverLicenseNumber';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DateOfBirth')
BEGIN
    ALTER TABLE AspNetUsers DROP COLUMN DateOfBirth;
    PRINT '  ? Dropped DateOfBirth';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Address')
BEGIN
    ALTER TABLE AspNetUsers DROP COLUMN Address;
    PRINT '  ? Dropped Address';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RegistrationDate')
BEGIN
    ALTER TABLE AspNetUsers DROP COLUMN RegistrationDate;
    PRINT '  ? Dropped RegistrationDate';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier')
BEGIN
    ALTER TABLE AspNetUsers DROP COLUMN Tier;
    PRINT '  ? Dropped Tier';
END

PRINT '';

-- 3. Verify Rentals still has CustomerId
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
BEGIN
    PRINT '? Rentals.CustomerId column exists (Good)';
    SELECT COUNT(*) as RentalCount FROM Rentals;
END
ELSE
BEGIN
    PRINT '? ERROR: Rentals.CustomerId column is missing!';
    PRINT '  Database may be in an inconsistent state.';
END

PRINT '';

-- 4. Verify Customers table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    PRINT '? Customers table exists (Good)';
    SELECT COUNT(*) as CustomerCount FROM Customers;
END
ELSE
BEGIN
    PRINT '? ERROR: Customers table is missing!';
    PRINT '  Database may be in an inconsistent state.';
END

PRINT '';
PRINT '===== CLEANUP COMPLETE =====';
PRINT 'Database should now be in a clean state ready for migration.';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Run: dotnet ef migrations add MigrateToApplicationUser';
PRINT '2. Review the migration file';
PRINT '3. Run: dotnet ef database update';
