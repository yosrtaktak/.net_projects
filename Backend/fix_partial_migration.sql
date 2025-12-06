-- Fix Partial Migration - Complete the MigrateToApplicationUser Migration
-- This script completes the migration that failed due to VehicleDamages table already existing

USE CarRentalDB;
GO

PRINT '========================================';
PRINT 'Fixing Partial Migration';
PRINT '========================================';
PRINT '';

-- Step 1: Check current state
PRINT 'Step 1: Checking current state...';
PRINT '';

-- Check if columns exist in AspNetUsers
PRINT 'Checking AspNetUsers columns:';
SELECT 
    CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Address') 
    THEN 'EXISTS' ELSE 'MISSING' END as Address,
    CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DateOfBirth') 
    THEN 'EXISTS' ELSE 'MISSING' END as DateOfBirth,
    CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber') 
    THEN 'EXISTS' ELSE 'MISSING' END as DriverLicenseNumber,
    CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RegistrationDate') 
    THEN 'EXISTS' ELSE 'MISSING' END as RegistrationDate,
    CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier') 
    THEN 'EXISTS' ELSE 'MISSING' END as Tier;

PRINT '';
PRINT 'Checking migration history:';
SELECT MigrationId, ProductVersion 
FROM __EFMigrationsHistory 
WHERE MigrationId LIKE '%MigrateToApplicationUser%';

PRINT '';
PRINT '----------------------------------------';

-- Step 2: Check if we need to manually add the columns
PRINT 'Step 2: Adding missing columns to AspNetUsers...';
PRINT '';

-- Add Address
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Address')
BEGIN
    ALTER TABLE AspNetUsers ADD Address NVARCHAR(500) NULL;
    PRINT '? Added Address column';
END
ELSE
    PRINT '? Address column already exists';

-- Add DateOfBirth
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DateOfBirth')
BEGIN
    ALTER TABLE AspNetUsers ADD DateOfBirth DATETIME2 NULL;
    PRINT '? Added DateOfBirth column';
END
ELSE
    PRINT '? DateOfBirth column already exists';

-- Add DriverLicenseNumber
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
BEGIN
    ALTER TABLE AspNetUsers ADD DriverLicenseNumber NVARCHAR(50) NULL;
    PRINT '? Added DriverLicenseNumber column';
END
ELSE
    PRINT '? DriverLicenseNumber column already exists';

-- Add RegistrationDate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RegistrationDate')
BEGIN
    ALTER TABLE AspNetUsers ADD RegistrationDate DATETIME2 NOT NULL DEFAULT ('0001-01-01T00:00:00.0000000');
    PRINT '? Added RegistrationDate column';
END
ELSE
    PRINT '? RegistrationDate column already exists';

-- Add Tier
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier')
BEGIN
    ALTER TABLE AspNetUsers ADD Tier INT NOT NULL DEFAULT 0;
    PRINT '? Added Tier column';
END
ELSE
    PRINT '? Tier column already exists';

PRINT '';
PRINT '----------------------------------------';

-- Step 3: Create indexes
PRINT 'Step 3: Creating indexes...';
PRINT '';

-- Create unique index on DriverLicenseNumber
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_DriverLicenseNumber' AND object_id = OBJECT_ID('AspNetUsers'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_AspNetUsers_DriverLicenseNumber 
    ON AspNetUsers(DriverLicenseNumber) 
    WHERE DriverLicenseNumber IS NOT NULL;
    PRINT '? Created IX_AspNetUsers_DriverLicenseNumber index';
END
ELSE
    PRINT '? IX_AspNetUsers_DriverLicenseNumber index already exists';

PRINT '';
PRINT '----------------------------------------';

-- Step 4: Seed data (if VehicleDamages table exists and needs data)
PRINT 'Step 4: Checking VehicleDamages seeding...';
PRINT '';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'VehicleDamages')
BEGIN
    PRINT '? VehicleDamages table exists';
    
    -- Check if seed data exists
    IF NOT EXISTS (SELECT * FROM VehicleDamages WHERE Id IN (1, 2, 3))
    BEGIN
        PRINT 'Adding seed data to VehicleDamages...';
        
        SET IDENTITY_INSERT VehicleDamages ON;
        
        -- Only insert if they don't exist
        IF NOT EXISTS (SELECT * FROM VehicleDamages WHERE Id = 1)
            INSERT INTO VehicleDamages (Id, Description, ImageUrl, RentalId, RepairCost, RepairedDate, ReportedBy, ReportedDate, Severity, Status, VehicleId)
            VALUES (1, 'Small scratch on rear bumper, likely from parking', NULL, NULL, 150.00, '2024-02-18', 'Customer', '2024-02-15', 0, 2, 1);
        
        IF NOT EXISTS (SELECT * FROM VehicleDamages WHERE Id = 2)
            INSERT INTO VehicleDamages (Id, Description, ImageUrl, RentalId, RepairCost, RepairedDate, ReportedBy, ReportedDate, Severity, Status, VehicleId)
            VALUES (2, 'Dent on driver''s side door, moderate damage', NULL, NULL, 450.00, '2024-03-20', 'Admin Staff', '2024-03-12', 1, 2, 1);
        
        IF NOT EXISTS (SELECT * FROM VehicleDamages WHERE Id = 3)
            INSERT INTO VehicleDamages (Id, Description, ImageUrl, RentalId, RepairCost, RepairedDate, ReportedBy, ReportedDate, Severity, Status, VehicleId)
            VALUES (3, 'Windshield chip from road debris', NULL, NULL, 200.00, NULL, 'Employee', '2024-04-10', 0, 1, 1);
        
        SET IDENTITY_INSERT VehicleDamages OFF;
        
        PRINT '? Seed data added to VehicleDamages';
    END
    ELSE
        PRINT '? VehicleDamages seed data already exists';
END
ELSE
BEGIN
    PRINT '? VehicleDamages table does not exist - this is unexpected';
END

PRINT '';
PRINT '----------------------------------------';

-- Step 5: Check Maintenances seed data
PRINT 'Step 5: Checking Maintenances seeding...';
PRINT '';

IF NOT EXISTS (SELECT * FROM Maintenances WHERE Id IN (1, 2, 3, 4))
BEGIN
    PRINT 'Adding seed data to Maintenances...';
    
    SET IDENTITY_INSERT Maintenances ON;
    
    IF NOT EXISTS (SELECT * FROM Maintenances WHERE Id = 1)
        INSERT INTO Maintenances (Id, CompletedDate, Cost, Description, ScheduledDate, Status, Type, VehicleId)
        VALUES (1, '2024-01-05', 85.00, 'Regular oil change and filter replacement', '2024-01-05', 2, 0, 1);
    
    IF NOT EXISTS (SELECT * FROM Maintenances WHERE Id = 2)
        INSERT INTO Maintenances (Id, CompletedDate, Cost, Description, ScheduledDate, Status, Type, VehicleId)
        VALUES (2, '2024-02-21', 320.00, 'Brake pad replacement and tire rotation', '2024-02-20', 2, 1, 1);
    
    IF NOT EXISTS (SELECT * FROM Maintenances WHERE Id = 3)
        INSERT INTO Maintenances (Id, CompletedDate, Cost, Description, ScheduledDate, Status, Type, VehicleId)
        VALUES (3, '2024-03-15', 50.00, 'Annual vehicle inspection', '2024-03-15', 2, 2, 1);
    
    IF NOT EXISTS (SELECT * FROM Maintenances WHERE Id = 4)
        INSERT INTO Maintenances (Id, CompletedDate, Cost, Description, ScheduledDate, Status, Type, VehicleId)
        VALUES (4, NULL, 150.00, 'Scheduled air conditioning service', '2024-05-01', 0, 0, 1);
    
    SET IDENTITY_INSERT Maintenances OFF;
    
    PRINT '? Seed data added to Maintenances';
END
ELSE
    PRINT '? Maintenances seed data already exists';

PRINT '';
PRINT '----------------------------------------';

-- Step 6: Update migration history
PRINT 'Step 6: Updating migration history...';
PRINT '';

IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251205141715_MigrateToApplicationUser')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251205141715_MigrateToApplicationUser', '8.0.0');
    PRINT '? Added migration record to history';
END
ELSE
    PRINT '? Migration record already exists in history';

PRINT '';
PRINT '========================================';
PRINT 'FINAL VERIFICATION';
PRINT '========================================';
PRINT '';

-- Verify all columns exist
PRINT 'AspNetUsers columns status:';
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AspNetUsers' 
AND COLUMN_NAME IN ('Address', 'DateOfBirth', 'DriverLicenseNumber', 'RegistrationDate', 'Tier')
ORDER BY COLUMN_NAME;

PRINT '';
PRINT 'Migration history:';
SELECT MigrationId, ProductVersion 
FROM __EFMigrationsHistory 
ORDER BY MigrationId;

PRINT '';
PRINT '========================================';
PRINT '? MIGRATION FIX COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Stop the backend if it''s running (Ctrl+C)';
PRINT '2. Run: dotnet clean';
PRINT '3. Run: dotnet build';
PRINT '4. Run: dotnet run';
PRINT '';

GO
