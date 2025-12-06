-- Simple Complete Migration
USE CarRentalDB;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT '========================================';
PRINT 'COMPLETE DATABASE MIGRATION';
PRINT '========================================';
PRINT '';

-- Step 1: Columns already added, verify
PRINT '1. Verifying columns...';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
    PRINT '   OK: DriverLicenseNumber';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier')
    PRINT '   OK: All customer columns exist';
PRINT '';

-- Step 2: Add UserId to Rentals
PRINT '2. Adding UserId to Rentals...';
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId')
BEGIN
    ALTER TABLE Rentals ADD UserId NVARCHAR(450) NULL;
    PRINT '   Added UserId column';
END
ELSE
    PRINT '   UserId already exists';
PRINT '';

-- Step 3: Migrate data from Customers if exists
PRINT '3. Migrating data...';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    -- Migrate customer data to AspNetUsers
    UPDATE u SET 
        u.DriverLicenseNumber = c.DriverLicenseNumber,
        u.DateOfBirth = c.DateOfBirth,
        u.Address = c.Address,
        u.RegistrationDate = c.RegistrationDate,
        u.Tier = c.Tier
    FROM AspNetUsers u
    INNER JOIN Customers c ON LOWER(u.Email) = LOWER(c.Email);
    PRINT '   Migrated customer data';
    
    -- Populate UserId in Rentals
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
    BEGIN
        UPDATE r SET r.UserId = u.Id
        FROM Rentals r
        INNER JOIN Customers c ON r.CustomerId = c.Id
        INNER JOIN AspNetUsers u ON LOWER(c.Email) = LOWER(u.Email)
        WHERE r.UserId IS NULL;
        PRINT '   Populated UserId in Rentals';
    END
END
ELSE
    PRINT '   No Customers table found';
PRINT '';

-- Step 4: Clean orphaned rentals
PRINT '4. Cleaning orphaned rentals...';
DELETE FROM Rentals WHERE UserId IS NULL;
PRINT '   Orphaned rentals removed';
PRINT '';

-- Step 5: Make UserId NOT NULL
PRINT '5. Setting UserId as required...';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId' AND is_nullable = 1)
BEGIN
    ALTER TABLE Rentals ALTER COLUMN UserId NVARCHAR(450) NOT NULL;
    PRINT '   UserId is now NOT NULL';
END
PRINT '';

-- Step 6: Drop old constraints
PRINT '6. Removing old constraints...';
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_Customers_CustomerId')
BEGIN
    ALTER TABLE Rentals DROP CONSTRAINT FK_Rentals_Customers_CustomerId;
    PRINT '   Dropped old FK';
END
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_CustomerId')
BEGIN
    DROP INDEX IX_Rentals_CustomerId ON Rentals;
    PRINT '   Dropped old index';
END
PRINT '';

-- Step 7: Drop CustomerId
PRINT '7. Removing CustomerId column...';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
BEGIN
    ALTER TABLE Rentals DROP COLUMN CustomerId;
    PRINT '   CustomerId removed';
END
PRINT '';

-- Step 8: Create new constraints
PRINT '8. Creating new constraints...';
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_UserId')
BEGIN
    CREATE INDEX IX_Rentals_UserId ON Rentals(UserId);
    PRINT '   Created UserId index';
END
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_DriverLicenseNumber')
BEGIN
    CREATE UNIQUE INDEX IX_AspNetUsers_DriverLicenseNumber ON AspNetUsers(DriverLicenseNumber) WHERE DriverLicenseNumber IS NOT NULL;
    PRINT '   Created DriverLicense index';
END
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_AspNetUsers_UserId')
BEGIN
    ALTER TABLE Rentals ADD CONSTRAINT FK_Rentals_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE NO ACTION;
    PRINT '   Created new FK';
END
PRINT '';

-- Step 9: Drop Customers table
PRINT '9. Removing Customers table...';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    DROP TABLE Customers;
    PRINT '   Customers table dropped';
END
PRINT '';

-- Step 10: Update migration history
PRINT '10. Updating migration history...';
DELETE FROM __EFMigrationsHistory WHERE MigrationId = '20251202180000_MigrateToApplicationUser';
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251205141715_MigrateToApplicationUser')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20251205141715_MigrateToApplicationUser', '9.0.0');
    PRINT '   Migration history updated';
END
PRINT '';

PRINT '========================================';
PRINT 'MIGRATION COMPLETE!';
PRINT '========================================';

-- Verification
SELECT 'Users' as TableName, COUNT(*) as Count FROM AspNetUsers
UNION ALL
SELECT 'Rentals', COUNT(*) FROM Rentals;

GO
