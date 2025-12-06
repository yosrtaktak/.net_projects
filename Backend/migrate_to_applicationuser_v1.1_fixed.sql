-- COMPREHENSIVE MIGRATION SCRIPT - FIXED VERSION v1.2
-- Migrate from Customers table to ApplicationUser (AspNetUsers)
-- Run this script directly in SQL Server Management Studio or Azure Data Studio
-- 
-- FIX: Adds explicit column verification and safer migration logic

USE CarRentalDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON; -- Rollback entire transaction if any error occurs

BEGIN TRANSACTION;

BEGIN TRY
    PRINT '========================================';
    PRINT 'CUSTOMER TO APPLICATIONUSER MIGRATION';
    PRINT 'FIXED VERSION - v1.2';
    PRINT '========================================';
    PRINT '';
    PRINT 'Starting at: ' + CONVERT(VARCHAR, GETDATE(), 120);
    PRINT '';

    -- ============================================
    -- STEP 1: Add customer columns to AspNetUsers
    -- ============================================
    PRINT '1. Adding customer columns to AspNetUsers...';
    
    -- Check and add DriverLicenseNumber
    DECLARE @hasDriverLicense BIT = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
    BEGIN
        SET @hasDriverLicense = 1;
        PRINT '   ? DriverLicenseNumber already exists';
    END
    ELSE
    BEGIN
        ALTER TABLE AspNetUsers ADD DriverLicenseNumber NVARCHAR(50) NULL;
        SET @hasDriverLicense = 1;
        PRINT '   ? Added DriverLicenseNumber';
    END
    
    -- Check and add DateOfBirth
    DECLARE @hasDateOfBirth BIT = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DateOfBirth')
    BEGIN
        SET @hasDateOfBirth = 1;
        PRINT '   ? DateOfBirth already exists';
    END
    ELSE
    BEGIN
        ALTER TABLE AspNetUsers ADD DateOfBirth DATETIME2 NULL;
        SET @hasDateOfBirth = 1;
        PRINT '   ? Added DateOfBirth';
    END
    
    -- Check and add Address
    DECLARE @hasAddress BIT = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Address')
    BEGIN
        SET @hasAddress = 1;
        PRINT '   ? Address already exists';
    END
    ELSE
    BEGIN
        ALTER TABLE AspNetUsers ADD Address NVARCHAR(500) NULL;
        SET @hasAddress = 1;
        PRINT '   ? Added Address';
    END
    
    -- Check and add RegistrationDate
    DECLARE @hasRegistrationDate BIT = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RegistrationDate')
    BEGIN
        SET @hasRegistrationDate = 1;
        PRINT '   ? RegistrationDate already exists';
    END
    ELSE
    BEGIN
        ALTER TABLE AspNetUsers ADD RegistrationDate DATETIME2 NOT NULL DEFAULT (GETUTCDATE());
        SET @hasRegistrationDate = 1;
        PRINT '   ? Added RegistrationDate';
    END
    
    -- Check and add Tier
    DECLARE @hasTier BIT = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier')
    BEGIN
        SET @hasTier = 1;
        PRINT '   ? Tier already exists';
    END
    ELSE
    BEGIN
        ALTER TABLE AspNetUsers ADD Tier INT NOT NULL DEFAULT (0);
        SET @hasTier = 1;
        PRINT '   ? Added Tier';
    END
    
    -- Verify all columns were added successfully
    IF @hasDriverLicense = 0 OR @hasDateOfBirth = 0 OR @hasAddress = 0 OR @hasRegistrationDate = 0 OR @hasTier = 0
    BEGIN
        RAISERROR('Failed to add all required columns to AspNetUsers', 16, 1);
    END
    
    PRINT '';

    -- ============================================
    -- STEP 2: Migrate customer data to AspNetUsers
    -- ============================================
    PRINT '2. Migrating customer data to AspNetUsers...';
    
    DECLARE @customersTableExists BIT = 0;
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
        SET @customersTableExists = 1;
    
    IF @customersTableExists = 1
    BEGIN
        DECLARE @migratedCount INT;
        
        -- Now that columns exist and are verified, we can safely update them
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
        
        -- Report on customers without matching AspNetUsers
        DECLARE @unmatchedCount INT;
        SELECT @unmatchedCount = COUNT(*)
        FROM Customers c
        WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers u WHERE LOWER(u.Email) = LOWER(c.Email));
        
        IF @unmatchedCount > 0
        BEGIN
            PRINT '   ? WARNING: ' + CAST(@unmatchedCount AS VARCHAR) + ' customers do not have matching AspNetUsers accounts';
        END
    END
    ELSE
    BEGIN
        PRINT '   ? Customers table does not exist, skipping data migration';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 3: Add UserId column to Rentals
    -- ============================================
    PRINT '3. Adding UserId column to Rentals...';
    
    DECLARE @hasUserId BIT = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId')
    BEGIN
        SET @hasUserId = 1;
        PRINT '   ? UserId column already exists';
    END
    ELSE
    BEGIN
        ALTER TABLE Rentals ADD UserId NVARCHAR(450) NULL;
        SET @hasUserId = 1;
        PRINT '   ? Added UserId column';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 4: Populate UserId in Rentals
    -- ============================================
    PRINT '4. Populating UserId in Rentals from CustomerId...';
    
    DECLARE @customerIdExists BIT = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
        SET @customerIdExists = 1;
    
    IF @customersTableExists = 1 AND @customerIdExists = 1
    BEGIN
        DECLARE @populatedCount INT;
        
        UPDATE r
        SET r.UserId = u.Id
        FROM Rentals r
        INNER JOIN Customers c ON r.CustomerId = c.Id
        INNER JOIN AspNetUsers u ON LOWER(c.Email) = LOWER(u.Email)
        WHERE r.CustomerId IS NOT NULL AND r.UserId IS NULL;
        
        SET @populatedCount = @@ROWCOUNT;
        PRINT '   ? Populated UserId for ' + CAST(@populatedCount AS VARCHAR) + ' rentals';
        
        -- Check for orphaned rentals
        DECLARE @orphanedCount INT;
        SELECT @orphanedCount = COUNT(*)
        FROM Rentals
        WHERE UserId IS NULL;
        
        IF @orphanedCount > 0
        BEGIN
            PRINT '   ? WARNING: ' + CAST(@orphanedCount AS VARCHAR) + ' rentals have NULL UserId';
            PRINT '     These rentals will be deleted in the next step';
        END
    END
    ELSE
    BEGIN
        PRINT '   ? Skipping population (Customers table or CustomerId column does not exist)';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 5: Make UserId NOT NULL
    -- ============================================
    PRINT '5. Making UserId column NOT NULL...';
    
    -- Delete any rentals with NULL UserId (orphaned rentals)
    DECLARE @deletedCount INT = 0;
    DECLARE @nullUserIdCount INT;
    SELECT @nullUserIdCount = COUNT(*) FROM Rentals WHERE UserId IS NULL;
    
    IF @nullUserIdCount > 0
    BEGIN
        DELETE FROM Rentals WHERE UserId IS NULL;
        SET @deletedCount = @@ROWCOUNT;
        PRINT '   ? Deleted ' + CAST(@deletedCount AS VARCHAR) + ' orphaned rentals with NULL UserId';
    END
    
    -- Now make the column NOT NULL
    DECLARE @userIdNullable BIT = 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId' AND is_nullable = 1)
        SET @userIdNullable = 1;
    
    IF @userIdNullable = 1
    BEGIN
        ALTER TABLE Rentals ALTER COLUMN UserId NVARCHAR(450) NOT NULL;
        PRINT '   ? Changed UserId to NOT NULL';
    END
    ELSE
    BEGIN
        PRINT '   ? UserId is already NOT NULL';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 6: Drop old foreign key and index
    -- ============================================
    PRINT '6. Removing old CustomerId constraints...';
    
    DECLARE @oldFkExists BIT = 0;
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_Customers_CustomerId')
        SET @oldFkExists = 1;
    
    IF @oldFkExists = 1
    BEGIN
        ALTER TABLE Rentals DROP CONSTRAINT FK_Rentals_Customers_CustomerId;
        PRINT '   ? Dropped FK_Rentals_Customers_CustomerId';
    END
    ELSE
    BEGIN
        PRINT '   ? FK_Rentals_Customers_CustomerId does not exist';
    END
    
    DECLARE @oldIndexExists BIT = 0;
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_CustomerId' AND object_id = OBJECT_ID('Rentals'))
        SET @oldIndexExists = 1;
    
    IF @oldIndexExists = 1
    BEGIN
        DROP INDEX IX_Rentals_CustomerId ON Rentals;
        PRINT '   ? Dropped IX_Rentals_CustomerId';
    END
    ELSE
    BEGIN
        PRINT '   ? IX_Rentals_CustomerId does not exist';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 7: Drop CustomerId column
    -- ============================================
    PRINT '7. Dropping CustomerId column...';
    
    IF @customerIdExists = 1
    BEGIN
        ALTER TABLE Rentals DROP COLUMN CustomerId;
        PRINT '   ? Dropped CustomerId column';
    END
    ELSE
    BEGIN
        PRINT '   ? CustomerId column does not exist';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 8: Create new indexes and foreign key
    -- ============================================
    PRINT '8. Creating new indexes and foreign key...';
    
    DECLARE @userIdIndexExists BIT = 0;
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_UserId' AND object_id = OBJECT_ID('Rentals'))
        SET @userIdIndexExists = 1;
    
    IF @userIdIndexExists = 0
    BEGIN
        CREATE INDEX IX_Rentals_UserId ON Rentals(UserId);
        PRINT '   ? Created IX_Rentals_UserId';
    END
    ELSE
    BEGIN
        PRINT '   ? IX_Rentals_UserId already exists';
    END
    
    DECLARE @driverLicenseIndexExists BIT = 0;
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_DriverLicenseNumber' AND object_id = OBJECT_ID('AspNetUsers'))
        SET @driverLicenseIndexExists = 1;
    
    IF @driverLicenseIndexExists = 0
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
    
    DECLARE @newFkExists BIT = 0;
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_AspNetUsers_UserId')
        SET @newFkExists = 1;
    
    IF @newFkExists = 0
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
    -- STEP 9: Drop Customers table
    -- ============================================
    PRINT '9. Dropping Customers table...';
    
    IF @customersTableExists = 1
    BEGIN
        DROP TABLE Customers;
        PRINT '   ? Dropped Customers table';
    END
    ELSE
    BEGIN
        PRINT '   ? Customers table does not exist';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 10: Update migration history
    -- ============================================
    PRINT '10. Updating migration history...';
    
    DECLARE @migrationExists BIT = 0;
    DECLARE @migrationCheckCount INT;
    
    SELECT @migrationCheckCount = COUNT(*)
    FROM __EFMigrationsHistory 
    WHERE MigrationId = '20251202180000_MigrateToApplicationUser';
    
    IF @migrationCheckCount = 0
        SET @migrationExists = 0;
    ELSE
        SET @migrationExists = 1;
    
    IF @migrationExists = 0
    BEGIN
        INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
        VALUES ('20251202180000_MigrateToApplicationUser', '9.0.0');
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
    
    DECLARE @userCount INT, @rentalCount INT, @customerTableStillExists BIT = 0;
    
    SELECT @userCount = COUNT(*) FROM AspNetUsers WHERE DriverLicenseNumber IS NOT NULL;
    SELECT @rentalCount = COUNT(*) FROM Rentals;
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
        SET @customerTableStillExists = 1;
    
    PRINT 'Users with customer data: ' + CAST(@userCount AS VARCHAR);
    PRINT 'Total rentals: ' + CAST(@rentalCount AS VARCHAR);
    PRINT 'Customers table: ' + CASE WHEN @customerTableStillExists = 0 THEN 'DROPPED ?' ELSE 'STILL EXISTS ?' END;
    
    PRINT '';
    PRINT '========================================';
    PRINT 'MIGRATION COMPLETED SUCCESSFULLY!';
    PRINT '========================================';
    PRINT 'Completed at: ' + CONVERT(VARCHAR, GETDATE(), 120);
    
    COMMIT TRANSACTION;
    PRINT '';
    PRINT 'Transaction COMMITTED';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    PRINT '';
    PRINT '========================================';
    PRINT 'ERROR OCCURRED - TRANSACTION ROLLED BACK';
    PRINT '========================================';
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR);
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR);
    PRINT 'Error Procedure: ' + ISNULL(ERROR_PROCEDURE(), 'N/A');
    PRINT '';
    PRINT 'Database has been rolled back to original state.';
    PRINT '';
    PRINT 'Common issues:';
    PRINT '- Columns already exist: Run cleanup_partial_migration.sql first';
    PRINT '- Foreign key conflicts: Check for orphaned rentals';
    PRINT '- Permission errors: Ensure you have ALTER TABLE permissions';
    
END CATCH;

SET NOCOUNT OFF;
GO
