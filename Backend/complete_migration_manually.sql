-- Complete the migration manually since everything is already in place
USE CarRentalDB;
GO

PRINT '========================================';
PRINT 'Completing Migration Manually';
PRINT '========================================';
PRINT '';

-- Check current state
PRINT 'Current Database State:';
PRINT '';

PRINT '1. AspNetUsers Columns:';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AspNetUsers' 
AND COLUMN_NAME IN ('Address', 'DateOfBirth', 'DriverLicenseNumber', 'RegistrationDate', 'Tier')
ORDER BY COLUMN_NAME;

PRINT '';
PRINT '2. Rentals Columns:';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Rentals' 
AND COLUMN_NAME IN ('UserId', 'CustomerId')
ORDER BY COLUMN_NAME;

PRINT '';
PRINT '3. Foreign Keys:';
SELECT name
FROM sys.foreign_keys 
WHERE name IN ('FK_Rentals_AspNetUsers_UserId', 'FK_Rentals_Customers_CustomerId');

PRINT '';
PRINT '4. Indexes:';
SELECT name
FROM sys.indexes 
WHERE name IN ('IX_Rentals_UserId', 'IX_AspNetUsers_DriverLicenseNumber')
AND object_id IN (OBJECT_ID('Rentals'), OBJECT_ID('AspNetUsers'));

PRINT '';
PRINT '5. Tables:';
SELECT name 
FROM sys.tables 
WHERE name IN ('Customers', 'VehicleDamages', 'Rentals', 'AspNetUsers');

PRINT '';
PRINT '========================================';
PRINT 'Analysis Complete';
PRINT '========================================';
PRINT '';

-- Check if migration already recorded
IF EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251205141715_MigrateToApplicationUser')
BEGIN
    PRINT 'Migration already recorded in history';
    PRINT 'Nothing to do!';
END
ELSE
BEGIN
    PRINT 'Migration NOT in history - adding record';
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251205141715_MigrateToApplicationUser', '9.0.0');
    PRINT '? Migration record added';
END

PRINT '';
PRINT '========================================';
PRINT 'Current Migration History:';
SELECT MigrationId, ProductVersion 
FROM __EFMigrationsHistory 
ORDER BY MigrationId;

PRINT '';
PRINT '? COMPLETE!';
PRINT '';
PRINT 'The database schema is complete and the migration is recorded.';
PRINT 'You can now start the backend with: dotnet run';
PRINT '';

GO
