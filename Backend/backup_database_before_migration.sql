-- =====================================================
-- DATABASE BACKUP SCRIPT
-- Before Running RemoveCustomersTable Migration
-- =====================================================
-- 
-- IMPORTANT: Run this script BEFORE running the migration!
-- This creates a backup of critical data from the Customers table
-- 
-- Usage:
-- 1. Open SQL Server Management Studio or Azure Data Studio
-- 2. Connect to your database
-- 3. Run this script to backup customer data
-- 4. Save the results to a file for safekeeping
-- =====================================================

USE CarRentalDB; -- Change to your database name
GO

-- Backup Customers table data
SELECT 
    'BACKUP: Customers Table' as BackupType,
    GETDATE() as BackupDate;

SELECT * 
INTO Customers_Backup_BeforeMigration
FROM Customers;

SELECT 
    COUNT(*) as TotalCustomers,
    'Customers backed up' as Message
FROM Customers_Backup_BeforeMigration;

-- Backup Rentals table (to verify relationships)
SELECT 
    'BACKUP: Rentals with Customer Info' as BackupType;

SELECT 
    r.*,
    c.Email as CustomerEmail,
    c.FirstName as CustomerFirstName,
    c.LastName as CustomerLastName
INTO Rentals_With_Customer_Backup
FROM Rentals r
LEFT JOIN Customers c ON r.CustomerId = c.Id;

SELECT 
    COUNT(*) as TotalRentals,
    COUNT(DISTINCT CustomerId) as UniqueCustomers,
    'Rentals backed up' as Message
FROM Rentals_With_Customer_Backup;

-- Show what will be affected by the migration
PRINT '===== MIGRATION IMPACT ANALYSIS =====';

SELECT 
    'Records in Customers table:' as Info,
    COUNT(*) as Count
FROM Customers
UNION ALL
SELECT 
    'Rentals linked to Customers:' as Info,
    COUNT(*) as Count
FROM Rentals
UNION ALL
SELECT 
    'Orphaned rentals (no customer):' as Info,
    COUNT(*) as Count
FROM Rentals r
LEFT JOIN Customers c ON r.CustomerId = c.Id
WHERE c.Id IS NULL;

PRINT 'Backup completed successfully!';
PRINT 'Backup tables created: Customers_Backup_BeforeMigration, Rentals_With_Customer_Backup';
GO
