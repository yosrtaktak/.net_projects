USE CarRentalDB;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Create unique index on DriverLicenseNumber
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_DriverLicenseNumber')
BEGIN
    CREATE UNIQUE INDEX IX_AspNetUsers_DriverLicenseNumber 
    ON AspNetUsers(DriverLicenseNumber) 
    WHERE DriverLicenseNumber IS NOT NULL;
    PRINT 'Created DriverLicenseNumber index';
END
GO

-- Create FK constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_AspNetUsers_UserId')
BEGIN
    ALTER TABLE Rentals 
    ADD CONSTRAINT FK_Rentals_AspNetUsers_UserId 
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id);
    PRINT 'Created FK constraint';
END
GO

-- Drop Customers table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    DROP TABLE Customers;
    PRINT 'Dropped Customers table';
END
GO

-- Update migration history
DELETE FROM __EFMigrationsHistory WHERE MigrationId LIKE '%MigrateToApplicationUser%';
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) 
VALUES ('20251205141715_MigrateToApplicationUser', '9.0.0');
PRINT 'Updated migration history';
GO

PRINT '';
PRINT '=== MIGRATION COMPLETE ===';
GO
