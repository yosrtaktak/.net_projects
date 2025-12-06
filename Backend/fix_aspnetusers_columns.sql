-- Fix script to ensure all AspNetUsers columns exist for the ApplicationUser migration
-- Run this if you're getting "Invalid column name 'DriverLicenseNumber'" errors

USE CarRentalDB;
GO

PRINT '=== FIXING ASPNETUSERS TABLE FOR APPLICATIONUSER MIGRATION ===';
PRINT '';

-- Add FirstName if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'FirstName')
BEGIN
    ALTER TABLE AspNetUsers ADD FirstName NVARCHAR(100) NULL;
    PRINT '? Added FirstName column';
END
ELSE
BEGIN
    PRINT '? FirstName column already exists';
END

-- Add LastName if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'LastName')
BEGIN
    ALTER TABLE AspNetUsers ADD LastName NVARCHAR(100) NULL;
    PRINT '? Added LastName column';
END
ELSE
BEGIN
    PRINT '? LastName column already exists';
END

-- Add CreatedAt if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE AspNetUsers ADD CreatedAt DATETIME2 NOT NULL DEFAULT (GETUTCDATE());
    PRINT '? Added CreatedAt column';
END
ELSE
BEGIN
    PRINT '? CreatedAt column already exists';
END

-- Add LastLogin if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'LastLogin')
BEGIN
    ALTER TABLE AspNetUsers ADD LastLogin DATETIME2 NULL;
    PRINT '? Added LastLogin column';
END
ELSE
BEGIN
    PRINT '? LastLogin column already exists';
END

-- Add DriverLicenseNumber if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
BEGIN
    ALTER TABLE AspNetUsers ADD DriverLicenseNumber NVARCHAR(50) NULL;
    PRINT '? Added DriverLicenseNumber column';
END
ELSE
BEGIN
    PRINT '? DriverLicenseNumber column already exists';
END

-- Add DateOfBirth if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DateOfBirth')
BEGIN
    ALTER TABLE AspNetUsers ADD DateOfBirth DATETIME2 NULL;
    PRINT '? Added DateOfBirth column';
END
ELSE
BEGIN
    PRINT '? DateOfBirth column already exists';
END

-- Add Address if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Address')
BEGIN
    ALTER TABLE AspNetUsers ADD Address NVARCHAR(500) NULL;
    PRINT '? Added Address column';
END
ELSE
BEGIN
    PRINT '? Address column already exists';
END

-- Add RegistrationDate if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RegistrationDate')
BEGIN
    ALTER TABLE AspNetUsers ADD RegistrationDate DATETIME2 NOT NULL DEFAULT (GETUTCDATE());
    PRINT '? Added RegistrationDate column';
END
ELSE
BEGIN
    PRINT '? RegistrationDate column already exists';
END

-- Add Tier if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier')
BEGIN
    ALTER TABLE AspNetUsers ADD Tier INT NOT NULL DEFAULT (0);
    PRINT '? Added Tier column';
END
ELSE
BEGIN
    PRINT '? Tier column already exists';
END

PRINT '';
PRINT '=== VERIFYING ALL COLUMNS ===';
PRINT '';

-- Verify all columns exist
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AspNetUsers'
AND COLUMN_NAME IN (
    'FirstName', 'LastName', 'CreatedAt', 'LastLogin',
    'DriverLicenseNumber', 'DateOfBirth', 'Address', 
    'RegistrationDate', 'Tier'
)
ORDER BY COLUMN_NAME;

PRINT '';
PRINT '=== CHECKING RENTALS TABLE ===';
PRINT '';

-- Check if Rentals has UserId column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId')
BEGIN
    PRINT '? WARNING: Rentals table does not have UserId column!';
    PRINT '  You may need to run the migration script to update Rentals.';
END
ELSE
BEGIN
    PRINT '? Rentals table has UserId column';
    
    -- Check if FK exists
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_AspNetUsers_UserId')
    BEGIN
        PRINT '? Foreign key FK_Rentals_AspNetUsers_UserId exists';
    END
    ELSE
    BEGIN
        PRINT '? WARNING: Foreign key FK_Rentals_AspNetUsers_UserId missing!';
        PRINT '  Creating foreign key...';
        
        ALTER TABLE Rentals 
        ADD CONSTRAINT FK_Rentals_AspNetUsers_UserId 
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id);
        
        PRINT '? Created foreign key FK_Rentals_AspNetUsers_UserId';
    END
END

-- Check if old CustomerId column still exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
BEGIN
    PRINT '? WARNING: Rentals table still has old CustomerId column!';
    PRINT '  This should be removed after migration is complete.';
END

PRINT '';
PRINT '=== CHECKING CUSTOMERS TABLE ===';
PRINT '';

-- Check if Customers table still exists
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Customers')
BEGIN
    PRINT '? WARNING: Customers table still exists!';
    PRINT '  This table should be removed after migration to ApplicationUser.';
    PRINT '';
    PRINT '  Customer count: ';
    SELECT COUNT(*) as CustomerCount FROM Customers;
END
ELSE
BEGIN
    PRINT '? Customers table has been removed (migration complete)';
END

PRINT '';
PRINT '=== FIX COMPLETE ===';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Restart your backend: dotnet run';
PRINT '2. Login again to get a new JWT token with correct claims';
PRINT '3. Test /api/users/me endpoint';
PRINT '';

GO
