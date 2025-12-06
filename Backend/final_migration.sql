-- Final Complete Migration - Step by Step
USE CarRentalDB;
GO

PRINT 'Starting migration...';
PRINT '';

-- 1. Add UserId column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId')
BEGIN
    ALTER TABLE Rentals ADD UserId NVARCHAR(450) NULL;
    PRINT '1. Added UserId column';
END
ELSE
    PRINT '1. UserId column already exists';

-- 2. Populate UserId from CustomerId + Customers + AspNetUsers
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    UPDATE r SET r.UserId = u.Id
    FROM Rentals r
    INNER JOIN Customers c ON r.CustomerId = c.Id
    INNER JOIN AspNetUsers u ON LOWER(c.Email) = LOWER(u.Email);
    PRINT '2. Populated UserId from Customers';
    
    -- Migrate customer data to AspNetUsers
    UPDATE u SET 
        u.DriverLicenseNumber = c.DriverLicenseNumber,
        u.DateOfBirth = c.DateOfBirth,
        u.Address = c.Address,
        u.RegistrationDate = c.RegistrationDate,
        u.Tier = c.Tier
    FROM AspNetUsers u
    INNER JOIN Customers c ON LOWER(u.Email) = LOWER(c.Email);
    PRINT '   Migrated customer data to AspNetUsers';
END
ELSE
    PRINT '2. No Customers table found';

-- 3. Delete orphaned rentals (if any)
DELETE FROM Rentals WHERE UserId IS NULL;
PRINT '3. Cleaned orphaned rentals';

-- 4. Make UserId NOT NULL
ALTER TABLE Rentals ALTER COLUMN UserId NVARCHAR(450) NOT NULL;
PRINT '4. Made UserId NOT NULL';

-- 5. Drop old FK constraint
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_Customers_CustomerId')
BEGIN
    ALTER TABLE Rentals DROP CONSTRAINT FK_Rentals_Customers_CustomerId;
    PRINT '5. Dropped old FK constraint';
END

-- 6. Drop old index
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_CustomerId')
BEGIN
    DROP INDEX IX_Rentals_CustomerId ON Rentals;
    PRINT '6. Dropped old index';
END

-- 7. Drop CustomerId column
ALTER TABLE Rentals DROP COLUMN CustomerId;
PRINT '7. Dropped CustomerId column';

-- 8. Create new index
CREATE INDEX IX_Rentals_UserId ON Rentals(UserId);
PRINT '8. Created UserId index';

-- 9. Create unique index on DriverLicenseNumber
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_DriverLicenseNumber')
BEGIN
    CREATE UNIQUE INDEX IX_AspNetUsers_DriverLicenseNumber ON AspNetUsers(DriverLicenseNumber) WHERE DriverLicenseNumber IS NOT NULL;
    PRINT '9. Created DriverLicenseNumber index';
END

-- 10. Create new FK constraint
ALTER TABLE Rentals ADD CONSTRAINT FK_Rentals_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE NO ACTION;
PRINT '10. Created new FK constraint';

-- 11. Drop Customers table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    DROP TABLE Customers;
    PRINT '11. Dropped Customers table';
END

-- 12. Update migration history
DELETE FROM __EFMigrationsHistory WHERE MigrationId LIKE '%MigrateToApplicationUser%';
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20251205141715_MigrateToApplicationUser', '9.0.0');
PRINT '12. Updated migration history';

PRINT '';
PRINT '=== MIGRATION COMPLETE ===';
PRINT '';

-- Show results
SELECT 'Total Users' as Info, COUNT(*) as Count FROM AspNetUsers
UNION ALL
SELECT 'Total Rentals', COUNT(*) FROM Rentals;

GO
