-- Create VehicleDamages table
USE CarRentalDB;
GO

PRINT '=== CREATING VEHICLEDAMAGES TABLE ===';
PRINT '';

-- Check if table already exists
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'VehicleDamages')
BEGIN
    PRINT '? VehicleDamages table already exists';
END
ELSE
BEGIN
    CREATE TABLE [dbo].[VehicleDamages] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [VehicleId] INT NOT NULL,
        [RentalId] INT NULL,
        [ReportedDate] DATETIME2 NOT NULL,
        [Description] NVARCHAR(MAX) NOT NULL DEFAULT(''),
        [Severity] INT NOT NULL,
        [RepairCost] DECIMAL(18,2) NOT NULL,
        [RepairedDate] DATETIME2 NULL,
        [ReportedBy] NVARCHAR(MAX) NULL,
        [ImageUrl] NVARCHAR(MAX) NULL,
        [Status] INT NOT NULL,
        
        CONSTRAINT [FK_VehicleDamages_Vehicles_VehicleId] 
            FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles]([Id]) ON DELETE CASCADE,
        
        CONSTRAINT [FK_VehicleDamages_Rentals_RentalId] 
            FOREIGN KEY ([RentalId]) REFERENCES [Rentals]([Id]) ON DELETE SET NULL
    );
    
    PRINT '? Created VehicleDamages table';
END

-- Add indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_VehicleDamages_VehicleId' AND object_id = OBJECT_ID('VehicleDamages'))
BEGIN
    CREATE INDEX [IX_VehicleDamages_VehicleId] ON [VehicleDamages]([VehicleId]);
    PRINT '? Created index IX_VehicleDamages_VehicleId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_VehicleDamages_RentalId' AND object_id = OBJECT_ID('VehicleDamages'))
BEGIN
    CREATE INDEX [IX_VehicleDamages_RentalId] ON [VehicleDamages]([RentalId]);
    PRINT '? Created index IX_VehicleDamages_RentalId';
END

PRINT '';
PRINT '=== VERIFYING TABLE STRUCTURE ===';
PRINT '';

-- Verify table structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'VehicleDamages'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '=== TABLE CREATION COMPLETE ===';
PRINT '';
PRINT 'You can now restart the backend and test damage reporting.';
PRINT '';

GO
