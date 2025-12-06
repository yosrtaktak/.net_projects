-- COMPREHENSIVE MIGRATION SCRIPT
-- Migrate from Customers table to ApplicationUser (AspNetUsers)
-- Run this script directly in SQL Server Management Studio or Azure Data Studio

USE CarRentalDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON; -- Rollback entire transaction if any error occurs

BEGIN TRANSACTION;

BEGIN TRY
    PRINT '========================================';
    PRINT 'CUSTOMER TO APPLICATIONUSER MIGRATION';
    PRINT '========================================';
    PRINT '';
    PRINT 'Starting at: ' + CONVERT(VARCHAR, GETDATE(), 120);
    PRINT '';

    -- ============================================
    -- STEP 1: Add customer columns to AspNetUsers
    -- ============================================
    PRINT '1. Adding customer columns to AspNetUsers...';
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
    BEGIN
        ALTER TABLE AspNetUsers ADD DriverLicenseNumber NVARCHAR(50) NULL;
        PRINT '   ? Added DriverLicenseNumber';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DateOfBirth')
    BEGIN
        ALTER TABLE AspNetUsers ADD DateOfBirth DATETIME2 NULL;
        PRINT '   ? Added DateOfBirth';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Address')
    BEGIN
        ALTER TABLE AspNetUsers ADD Address NVARCHAR(500) NULL;
        PRINT '   ? Added Address';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RegistrationDate')
    BEGIN
        ALTER TABLE AspNetUsers ADD RegistrationDate DATETIME2 NOT NULL DEFAULT (GETUTCDATE());
        PRINT '   ? Added RegistrationDate';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier')
    BEGIN
        ALTER TABLE AspNetUsers ADD Tier INT NOT NULL DEFAULT (0);
        PRINT '   ? Added Tier';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 2: Migrate customer data to AspNetUsers
    -- ============================================
    PRINT '2. Migrating customer data to AspNetUsers...';
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
    BEGIN
        DECLARE @migratedCount INT;
        
        UPDATE u
        SET 
            u.FirstName = ISNULL(c.FirstName, u.UserName),
            u.LastName = ISNULL(c.LastName, ''),
            u.PhoneNumber = ISNULL(c.PhoneNumber, ''),
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
        PRINT '   ? Customers table does not exist, skipping data migration';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 3: Add UserId column to Rentals
    -- ============================================
    PRINT '3. Adding UserId column to Rentals...';
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId')
    BEGIN
        ALTER TABLE Rentals ADD UserId NVARCHAR(450) NULL;
        PRINT '   ? Added UserId column';
    END
    ELSE
    BEGIN
        PRINT '   ? UserId column already exists';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 4: Populate UserId in Rentals
    -- ============================================
    PRINT '4. Populating UserId in Rentals from CustomerId...';
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
       AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
    BEGIN
        DECLARE @populatedCount INT;
        
        UPDATE r
        SET r.UserId = u.Id
        FROM Rentals r
        INNER JOIN Customers c ON r.CustomerId = c.Id
        INNER JOIN AspNetUsers u ON LOWER(c.Email) = LOWER(u.Email)
        WHERE r.CustomerId IS NOT NULL;
        
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
            PRINT '     These rentals will need to be fixed manually or deleted';
        END
    END
    
    PRINT '';

    -- ============================================
    -- STEP 5: Make UserId NOT NULL
    -- ============================================
    PRINT '5. Making UserId column NOT NULL...';
    
    -- Delete any rentals with NULL UserId (orphaned rentals)
    DECLARE @deletedCount INT = 0;
    IF EXISTS (SELECT * FROM Rentals WHERE UserId IS NULL)
    BEGIN
        DELETE FROM Rentals WHERE UserId IS NULL;
        SET @deletedCount = @@ROWCOUNT;
        PRINT '   ? Deleted ' + CAST(@deletedCount AS VARCHAR) + ' orphaned rentals with NULL UserId';
    END
    
    -- Now make the column NOT NULL
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId' AND is_nullable = 1)
    BEGIN
        ALTER TABLE Rentals ALTER COLUMN UserId NVARCHAR(450) NOT NULL;
        PRINT '   ? Changed UserId to NOT NULL';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 6: Drop old foreign key and index
    -- ============================================
    PRINT '6. Removing old CustomerId constraints...';
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_Customers_CustomerId')
    BEGIN
        ALTER TABLE Rentals DROP CONSTRAINT FK_Rentals_Customers_CustomerId;
        PRINT '   ? Dropped FK_Rentals_Customers_CustomerId';
    END
    
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_CustomerId' AND object_id = OBJECT_ID('Rentals'))
    BEGIN
        DROP INDEX IX_Rentals_CustomerId ON Rentals;
        PRINT '   ? Dropped IX_Rentals_CustomerId';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 7: Drop CustomerId column
    -- ============================================
    PRINT '7. Dropping CustomerId column...';
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
    BEGIN
        ALTER TABLE Rentals DROP COLUMN CustomerId;
        PRINT '   ? Dropped CustomerId column';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 8: Create new indexes and foreign key
    -- ============================================
    PRINT '8. Creating new indexes and foreign key...';
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_UserId' AND object_id = OBJECT_ID('Rentals'))
    BEGIN
        CREATE INDEX IX_Rentals_UserId ON Rentals(UserId);
        PRINT '   ? Created IX_Rentals_UserId';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_DriverLicenseNumber' AND object_id = OBJECT_ID('AspNetUsers'))
    BEGIN
        CREATE UNIQUE INDEX IX_AspNetUsers_DriverLicenseNumber 
        ON AspNetUsers(DriverLicenseNumber) 
        WHERE DriverLicenseNumber IS NOT NULL;
        PRINT '   ? Created IX_AspNetUsers_DriverLicenseNumber';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_AspNetUsers_UserId')
    BEGIN
        ALTER TABLE Rentals 
        ADD CONSTRAINT FK_Rentals_AspNetUsers_UserId 
        FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) 
        ON DELETE NO ACTION;
        PRINT '   ? Created FK_Rentals_AspNetUsers_UserId';
    END
    
    PRINT '';

    -- ============================================
    -- STEP 9: Drop Customers table
    -- ============================================
    PRINT '9. Dropping Customers table...';
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
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
    
    IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251202180000_MigrateToApplicationUser')
    BEGIN
        INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
        VALUES ('20251202180000_MigrateToApplicationUser', '9.0.0');
        PRINT '   ? Added migration record';
    END
    
    PRINT '';

    -- ============================================
    -- VERIFICATION
    -- ============================================
    PRINT '========================================';
    PRINT 'VERIFICATION';
    PRINT '========================================';
    
    DECLARE @userCount INT, @rentalCount INT, @customerCount INT = 0;
    
    SELECT @userCount = COUNT(*) FROM AspNetUsers WHERE DriverLicenseNumber IS NOT NULL;
    SELECT @rentalCount = COUNT(*) FROM Rentals;
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
        SELECT @customerCount = COUNT(*) FROM Customers;
    
    PRINT 'Users with customer data: ' + CAST(@userCount AS VARCHAR);
    PRINT 'Total rentals: ' + CAST(@rentalCount AS VARCHAR);
    PRINT 'Customers table: ' + CASE WHEN @customerCount = 0 THEN 'DROPPED ?' ELSE 'STILL EXISTS ?' END;
    
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
    PRINT '';
    PRINT 'Database has been rolled back to original state.';
    
END CATCH;

SET NOCOUNT OFF;
GO
