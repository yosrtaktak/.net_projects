-- Diagnostic script to check database state and user authentication issues
USE CarRentalDB;
GO

PRINT '=== CHECKING ASPNETUSERS TABLE STRUCTURE ===';
PRINT '';

-- Check if custom columns exist
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AspNetUsers'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '=== CHECKING USERS AND ROLES ===';
PRINT '';

-- Check users and their roles
SELECT 
    u.Id,
    u.UserName,
    u.Email,
    u.EmailConfirmed,
    u.FirstName,
    u.LastName,
    r.Name as RoleName,
    CASE 
        WHEN EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
        THEN 'Column Exists'
        ELSE 'Column Missing'
    END as DriverLicenseColumn
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
ORDER BY u.Email;

PRINT '';
PRINT '=== CHECKING RENTALS STRUCTURE ===';
PRINT '';

-- Check Rentals foreign keys
SELECT 
    fk.name as ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) as TableName,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) as ColumnName,
    OBJECT_NAME(fk.referenced_object_id) as ReferencedTable,
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) as ReferencedColumn
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fc ON fk.object_id = fc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'Rentals';

PRINT '';
PRINT '=== CHECKING IF CUSTOMERS TABLE EXISTS ===';
PRINT '';

SELECT 
    CASE 
        WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Customers')
        THEN 'Customers table exists (BAD - should be removed)'
        ELSE 'Customers table removed (GOOD)'
    END as Status;

GO
