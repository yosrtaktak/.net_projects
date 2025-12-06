-- Complete Migration Script
-- Migrate from Customers to ApplicationUser
-- Fixed for SQL Server settings

USE CarRentalDB;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

BEGIN TRY
    PRINT '========================================';
    PRINT 'COMPLETE DATABASE MIGRATION';
    PRINT '========================================';
    PRINT 'Started: ' + CONVERT(VARCHAR, GETDATE(), 120);
    PRINT '';

    -- ============================================
    -- STEP 1: Verify columns exist (already added)
    -- ============================================
    PRINT '1. Verifying AspNetUsers columns...';
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
        PRINT '   ? DriverLicenseNumber exists';
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DateOfBirth')
        PRINT '   ? DateOfBirth exists';
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Address')
        PRINT '   ? Address exists';
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RegistrationDate')
        PRINT '   ? RegistrationDate exists';
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier')
        PRINT '   ? Tier exists';
    
    PRINT '';

    -- ============================================
    -- STEP 2: Migrate customer data if Customers table exists
    -- ============================================
    PRINT '2. Checking for customer data to migrate...';
    
    DECLARE @customersExists BIT = 0;
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
    BEGIN
        SET @customersExists = 1;
        PRINT '   ? Customers table found';
        
        -- Migrate data
        DECLARE @migratedCount INT;
        UPDATE u
        SET 
            u.FirstName = ISNULL(c.FirstName, u.FirstName),
            u.LastName = ISNULL(c.LastName, u.LastName),
            u.PhoneNumber = ISNULL(c.PhoneNumber, u.PhoneNumber),
            u.DriverLicenseNumber = c.DriverLicenseNumber,
            u.DateOfBirth = c.DateOfBirth,
            u.Address = c.Address,
            u.RegistrationDate = c.RegistrationDate,
            u.Tier = c.Tier
        FROM AspNetUsers u
        INNER JOIN Customers c ON LOWER(u.Email) = LOWER(c.Email);
        
        SET @migratedCount = @@ROWCOUNT;
        PRINT '   ? Migrated ' + CAST(@migratedCount AS VARCHAR) + ' customer records';
    END
    ELSE
    BEGIN
        PRINT '   ? No Customers table found (already migrated or fresh install)';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 3: Add UserId to Rentals if needed
    -- ============================================
    PRINT '3. Updating Rentals table...';
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId')
    BEGIN
        ALTER TABLE Rentals ADD UserId NVARCHAR(450) NULL;
        PRINT '   ? Added UserId column';
    END
    ELSE
    BEGIN
        PRINT '   ? UserId column already exists';
    END
    
    -- Populate UserId if Customers table exists
    IF @customersExists = 1 AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
    BEGIN
        DECLARE @populatedCount INT;
        UPDATE r
        SET r.UserId = u.Id
        FROM Rentals r
        INNER JOIN Customers c ON r.CustomerId = c.Id
        INNER JOIN AspNetUsers u ON LOWER(c.Email) = LOWER(u.Email)
        WHERE r.UserId IS NULL;
        
        SET @populatedCount = @@ROWCOUNT;
        PRINT '   ? Populated UserId for ' + CAST(@populatedCount AS VARCHAR) + ' rentals';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 4: Clean up orphaned rentals and make UserId NOT NULL
    -- ============================================
    PRINT '4. Finalizing Rentals structure...';
    
    -- Delete orphaned rentals
    DECLARE @orphanedCount INT;
    SELECT @orphanedCount = COUNT(*) FROM Rentals WHERE UserId IS NULL;
    
    IF @orphanedCount > 0
    BEGIN
        DELETE FROM Rentals WHERE UserId IS NULL;
        PRINT '   ? Deleted ' + CAST(@orphanedCount AS VARCHAR) + ' orphaned rentals';
    END
    
    -- Make UserId NOT NULL
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId' AND is_nullable = 1)
    BEGIN
        ALTER TABLE Rentals ALTER COLUMN UserId NVARCHAR(450) NOT NULL;
        PRINT '   ? Made UserId NOT NULL';
    END
    ELSE
    BEGIN
        PRINT '   ? UserId already NOT NULL';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 5: Remove old CustomerId references
    -- ============================================
    PRINT '5. Removing old CustomerId references...';
    
    -- Drop foreign key
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_Customers_CustomerId')
    BEGIN
        ALTER TABLE Rentals DROP CONSTRAINT FK_Rentals_Customers_CustomerId;
        PRINT '   ? Dropped FK_Rentals_Customers_CustomerId';
    END
    
    -- Drop index
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_CustomerId' AND object_id = OBJECT_ID('Rentals'))
    BEGIN
        DROP INDEX IX_Rentals_CustomerId ON Rentals;
        PRINT '   ? Dropped IX_Rentals_CustomerId';
    END
    
    -- Drop column
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
    BEGIN
        ALTER TABLE Rentals DROP COLUMN CustomerId;
        PRINT '   ? Dropped CustomerId column';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 6: Create new constraints and indexes
    -- ============================================
    PRINT '6. Creating new constraints...';
    
    -- Create UserId index
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_UserId' AND object_id = OBJECT_ID('Rentals'))
    BEGIN
        CREATE INDEX IX_Rentals_UserId ON Rentals(UserId);
        PRINT '   ? Created IX_Rentals_UserId';
    END
    ELSE
    BEGIN
        PRINT '   ? IX_Rentals_UserId already exists';
    END
    
    -- Create DriverLicenseNumber unique index
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_DriverLicenseNumber' AND object_id = OBJECT_ID('AspNetUsers'))
    BEGIN
        CREATE UNIQUE INDEX IX_AspNetUsers_DriverLicenseNumber 
        ON AspNetUsers(DriverLicenseNumber) 
        WHERE DriverLicenseNumber IS NOT NULL;
        PRINT '   ? Created IX_AspNetUsers_DriverLicenseNumber';
    END
    ELSE
    BEGIN
        PRINT '   ? IX_AspNetUsers_DriverLicenseNumber already exists';
    END
    
    -- Create foreign key
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_AspNetUsers_UserId')
    BEGIN
        ALTER TABLE Rentals 
        ADD CONSTRAINT FK_Rentals_AspNetUsers_UserId 
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) 
        ON DELETE NO ACTION;
        PRINT '   ? Created FK_Rentals_AspNetUsers_UserId';
    END
    ELSE
    BEGIN
        PRINT '   ? FK_Rentals_AspNetUsers_UserId already exists';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 7: Drop Customers table
    -- ============================================
    PRINT '7. Cleaning up old tables...';
    
    IF @customersExists = 1
    BEGIN
        DROP TABLE Customers;
        PRINT '   ? Dropped Customers table';
    END
    ELSE
    BEGIN
        PRINT '   ? Customers table already dropped';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 8: Update migration history
    -- ============================================
    PRINT '8. Updating migration history...';
    
    -- Remove any conflicting migration records
    DELETE FROM __EFMigrationsHistory WHERE MigrationId = '20251202180000_MigrateToApplicationUser';
    
    -- Add the correct migration record
    IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251205141715_MigrateToApplicationUser')
    BEGIN
        INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
        VALUES ('20251205141715_MigrateToApplicationUser', '9.0.0');
        PRINT '   ? Added migration record';
    END
    ELSE
    BEGIN
        PRINT '   ? Migration record already exists';
    END
    
    PRINT '';

    -- ============================================
    -- VERIFICATION
    -- ============================================
    PRINT '========================================';
    PRINT 'VERIFICATION';
    PRINT '========================================';
    
    DECLARE @userCount INT, @rentalCount INT;
    SELECT @userCount = COUNT(*) FROM AspNetUsers;
    SELECT @rentalCount = COUNT(*) FROM Rentals;
    
    PRINT 'Total users: ' + CAST(@userCount AS VARCHAR);
    PRINT 'Total rentals: ' + CAST(@rentalCount AS VARCHAR);
    PRINT 'Customers table: ' + CASE WHEN EXISTS(SELECT * FROM sys.tables WHERE name = 'Customers') THEN 'EXISTS ?' ELSE 'DROPPED ?' END;
    PRINT 'UserId column: ' + CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId') THEN 'EXISTS ?' ELSE 'MISSING ?' END;
    PRINT 'CustomerId column: ' + CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId') THEN 'EXISTS ?' ELSE 'REMOVED ?' END;
    
    PRINT '';
    PRINT '========================================';
    PRINT 'MIGRATION COMPLETED SUCCESSFULLY!';
    PRINT '========================================';
    PRINT 'Completed: ' + CONVERT(VARCHAR, GETDATE(), 120);
    
    COMMIT TRANSACTION;
    PRINT '';
    PRINT '? Transaction COMMITTED';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT '';
    PRINT '========================================';
    PRINT '? ERROR - TRANSACTION ROLLED BACK';
    PRINT '========================================';
    PRINT 'Error: ' + ERROR_MESSAGE();
    PRINT 'Line: ' + CAST(ERROR_LINE() AS VARCHAR);
    
END CATCH;

SET NOCOUNT OFF;
GO
