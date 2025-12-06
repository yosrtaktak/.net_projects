-- Fix UserId Foreign Key Constraint Issue
-- The problem: Rentals.UserId has empty strings, but AspNetUsers.Id never has empty strings
USE CarRentalDB;
GO

PRINT '========================================';
PRINT 'Fixing UserId Foreign Key Issue';
PRINT '========================================';
PRINT '';

-- Step 1: Check current state
PRINT 'Step 1: Checking Rentals table...';
SELECT 
    COUNT(*) as TotalRentals,
    COUNT(CASE WHEN UserId = '' THEN 1 END) as EmptyUserIdCount,
    COUNT(CASE WHEN UserId IS NULL THEN 1 END) as NullUserIdCount,
    COUNT(CASE WHEN UserId != '' AND UserId IS NOT NULL THEN 1 END) as ValidUserIdCount
FROM Rentals;

PRINT '';
PRINT 'Step 2: Checking AspNetUsers...';
SELECT COUNT(*) as TotalUsers FROM AspNetUsers;

PRINT '';
PRINT '----------------------------------------';

-- Step 2: Drop the foreign key if it exists
PRINT 'Step 3: Removing existing foreign key (if exists)...';
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_AspNetUsers_UserId')
BEGIN
    ALTER TABLE Rentals DROP CONSTRAINT FK_Rentals_AspNetUsers_UserId;
    PRINT '? Foreign key dropped';
END
ELSE
    PRINT '? Foreign key does not exist';

PRINT '';
PRINT '----------------------------------------';

-- Step 3: Make UserId nullable temporarily to fix the data
PRINT 'Step 4: Making UserId nullable...';
IF EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('Rentals') 
    AND name = 'UserId' 
    AND is_nullable = 0
)
BEGIN
    -- First, update empty strings to NULL
    PRINT 'Updating empty UserId values to NULL...';
    UPDATE Rentals SET UserId = NULL WHERE UserId = '';
    PRINT CONCAT('? Updated ', @@ROWCOUNT, ' rows');
    
    -- Now make it nullable
    ALTER TABLE Rentals ALTER COLUMN UserId NVARCHAR(450) NULL;
    PRINT '? UserId column is now nullable';
END
ELSE
    PRINT '? UserId is already nullable';

PRINT '';
PRINT '----------------------------------------';

-- Step 4: Verify no orphaned rentals
PRINT 'Step 5: Checking for orphaned rentals...';
SELECT 
    r.Id as RentalId,
    r.UserId,
    CASE WHEN u.Id IS NULL THEN 'ORPHANED' ELSE 'OK' END as Status
FROM Rentals r
LEFT JOIN AspNetUsers u ON r.UserId = u.Id
WHERE r.UserId IS NOT NULL;

DECLARE @OrphanCount INT;
SELECT @OrphanCount = COUNT(*)
FROM Rentals r
LEFT JOIN AspNetUsers u ON r.UserId = u.Id
WHERE r.UserId IS NOT NULL AND u.Id IS NULL;

IF @OrphanCount > 0
BEGIN
    PRINT CONCAT('? Warning: Found ', @OrphanCount, ' orphaned rentals');
    PRINT 'Setting orphaned UserId values to NULL...';
    UPDATE r
    SET r.UserId = NULL
    FROM Rentals r
    LEFT JOIN AspNetUsers u ON r.UserId = u.Id
    WHERE r.UserId IS NOT NULL AND u.Id IS NULL;
    PRINT CONCAT('? Fixed ', @@ROWCOUNT, ' orphaned rentals');
END
ELSE
    PRINT '? No orphaned rentals found';

PRINT '';
PRINT '----------------------------------------';

-- Step 5: Recreate the foreign key with nullable support
PRINT 'Step 6: Creating foreign key constraint...';
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_AspNetUsers_UserId')
BEGIN
    ALTER TABLE Rentals 
    ADD CONSTRAINT FK_Rentals_AspNetUsers_UserId 
    FOREIGN KEY (UserId) 
    REFERENCES AspNetUsers(Id)
    ON DELETE NO ACTION;
    PRINT '? Foreign key constraint created';
END
ELSE
    PRINT '? Foreign key already exists';

PRINT '';
PRINT '========================================';
PRINT 'Verification';
PRINT '========================================';
PRINT '';

-- Verify final state
PRINT 'Final Rentals state:';
SELECT 
    COUNT(*) as TotalRentals,
    COUNT(CASE WHEN UserId IS NULL THEN 1 END) as NullUserId,
    COUNT(CASE WHEN UserId IS NOT NULL THEN 1 END) as HasUserId
FROM Rentals;

PRINT '';
PRINT 'Foreign Key status:';
SELECT 
    name as ConstraintName,
    OBJECT_NAME(parent_object_id) as TableName,
    OBJECT_NAME(referenced_object_id) as ReferencedTable
FROM sys.foreign_keys 
WHERE name = 'FK_Rentals_AspNetUsers_UserId';

PRINT '';
PRINT '? COMPLETE!';
PRINT '';
PRINT 'The UserId foreign key issue is fixed.';
PRINT 'You can now restart the backend.';
PRINT '';

GO
