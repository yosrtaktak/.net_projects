-- Remove PriceMultiplier column from Categories table
-- This field is not being used in pricing calculations

USE CarRentalDB;
GO

-- Check if the column exists before dropping
IF EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Categories' 
    AND COLUMN_NAME = 'PriceMultiplier'
)
BEGIN
    PRINT 'Removing PriceMultiplier column from Categories table...'
    
    ALTER TABLE Categories
    DROP COLUMN PriceMultiplier;
    
    PRINT 'PriceMultiplier column removed successfully!'
END
ELSE
BEGIN
    PRINT 'PriceMultiplier column does not exist in Categories table.'
END
GO

-- Verify the schema after change
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Categories'
ORDER BY ORDINAL_POSITION;
GO

PRINT 'Categories table schema updated successfully!'
