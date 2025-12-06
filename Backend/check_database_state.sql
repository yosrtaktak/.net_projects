-- Check current database state before migration
USE CarRentalDB;
GO

PRINT '===== DATABASE STATE CHECK =====';
PRINT '';

-- Check if Customers table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    PRINT '? Customers table EXISTS';
    SELECT COUNT(*) as CustomerCount FROM Customers;
END
ELSE
BEGIN
    PRINT '? Customers table does NOT exist';
END

PRINT '';

-- Check Rentals table structure
PRINT 'Rentals table columns:';
SELECT 
    c.name as ColumnName,
    t.name as DataType,
    c.is_nullable as IsNullable
FROM sys.columns c
JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('Rentals')
ORDER BY c.column_id;

PRINT '';

-- Check if Rentals has CustomerId or UserId
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
BEGIN
    PRINT '? Rentals has CustomerId column';
    SELECT COUNT(*) as RentalsWithCustomerId FROM Rentals WHERE CustomerId IS NOT NULL;
END
ELSE
BEGIN
    PRINT '? Rentals does NOT have CustomerId column';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId')
BEGIN
    PRINT '? Rentals has UserId column';
    SELECT COUNT(*) as RentalsWithUserId FROM Rentals WHERE UserId IS NOT NULL;
END
ELSE
BEGIN
    PRINT '? Rentals does NOT have UserId column';
END

PRINT '';

-- Check AspNetUsers columns
PRINT 'AspNetUsers custom columns:';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
    PRINT '? Has DriverLicenseNumber';
ELSE
    PRINT '? Missing DriverLicenseNumber';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier')
    PRINT '? Has Tier';
ELSE
    PRINT '? Missing Tier';

PRINT '';

-- Check for orphaned rentals
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    PRINT 'Checking for orphaned rentals...';
    SELECT 
        COUNT(*) as OrphanedRentals
    FROM Rentals r
    LEFT JOIN Customers c ON r.CustomerId = c.Id
    WHERE c.Id IS NULL;
END

PRINT '';

-- Check applied migrations
PRINT 'Applied migrations:';
SELECT MigrationId, ProductVersion
FROM __EFMigrationsHistory
ORDER BY MigrationId;

PRINT '';
PRINT '===== END CHECK =====';
