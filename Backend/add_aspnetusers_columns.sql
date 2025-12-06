-- Add customer columns to AspNetUsers
-- Simple version - just adds the columns

USE CarRentalDB;
GO

PRINT 'Adding customer columns to AspNetUsers...';
PRINT '';

-- Add DriverLicenseNumber
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
BEGIN
    ALTER TABLE AspNetUsers ADD DriverLicenseNumber NVARCHAR(50) NULL;
    PRINT '? Added DriverLicenseNumber';
END
ELSE
BEGIN
    PRINT '? DriverLicenseNumber already exists';
END

-- Add DateOfBirth
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DateOfBirth')
BEGIN
    ALTER TABLE AspNetUsers ADD DateOfBirth DATETIME2 NULL;
    PRINT '? Added DateOfBirth';
END
ELSE
BEGIN
    PRINT '? DateOfBirth already exists';
END

-- Add Address
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Address')
BEGIN
    ALTER TABLE AspNetUsers ADD Address NVARCHAR(500) NULL;
    PRINT '? Added Address';
END
ELSE
BEGIN
    PRINT '? Address already exists';
END

-- Add RegistrationDate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RegistrationDate')
BEGIN
    ALTER TABLE AspNetUsers ADD RegistrationDate DATETIME2 NOT NULL DEFAULT (GETUTCDATE());
    PRINT '? Added RegistrationDate';
END
ELSE
BEGIN
    PRINT '? RegistrationDate already exists';
END

-- Add Tier
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier')
BEGIN
    ALTER TABLE AspNetUsers ADD Tier INT NOT NULL DEFAULT (0);
    PRINT '? Added Tier';
END
ELSE
BEGIN
    PRINT '? Tier already exists';
END

PRINT '';
PRINT '? Column additions complete!';
PRINT '';
PRINT 'Verifying columns...';

SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AspNetUsers' 
AND COLUMN_NAME IN ('DriverLicenseNumber', 'DateOfBirth', 'Address', 'RegistrationDate', 'Tier')
ORDER BY COLUMN_NAME;

PRINT '';
PRINT 'Done! You can now apply the EF Core migration with: dotnet ef database update';
GO
