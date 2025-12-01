-- Vehicle History Test Data Seed Script
-- Run this script to populate test data for Vehicle ID 1 (Toyota Corolla)

USE [CarRentalDB]; -- Database name from appsettings.json
GO

BEGIN TRANSACTION;

BEGIN TRY
    -- Update vehicle mileage
    UPDATE Vehicles 
    SET Mileage = 16000 
    WHERE Id = 1;

    -- Check if rentals already exist
    IF NOT EXISTS (SELECT 1 FROM Rentals WHERE Id IN (1, 2, 3, 4))
    BEGIN
        -- Enable identity insert for Rentals
        SET IDENTITY_INSERT Rentals ON;

        -- Insert 4 rental history records
        INSERT INTO Rentals (Id, CustomerId, VehicleId, StartDate, EndDate, ActualReturnDate, TotalCost, Status, StartMileage, EndMileage, Notes, CreatedAt)
        VALUES 
        (1, 1, 1, '2024-01-15', '2024-01-20', '2024-01-20', 175.00, 2, 15000, 15250, 'Regular rental, vehicle returned in good condition', '2024-01-10'),
        (2, 1, 1, '2024-02-10', '2024-02-15', '2024-02-15', 175.00, 2, 15250, 15480, 'Weekend trip rental', '2024-02-05'),
        (3, 1, 1, '2024-03-05', '2024-03-12', '2024-03-12', 245.00, 2, 15480, 15820, 'Business trip rental', '2024-03-01'),
        (4, 1, 1, '2024-04-01', '2024-04-05', '2024-04-06', 175.00, 2, 15820, 16000, 'Returned 1 day late, extra charges applied', '2024-03-28');

        SET IDENTITY_INSERT Rentals OFF;
        
        PRINT '✓ Inserted 4 rental records';
    END
    ELSE
    BEGIN
        PRINT 'ℹ Rental records already exist, skipping...';
    END

    -- Check if maintenance records already exist
    IF NOT EXISTS (SELECT 1 FROM Maintenances WHERE Id IN (1, 2, 3, 4))
    BEGIN
        -- Enable identity insert for Maintenances
        SET IDENTITY_INSERT Maintenances ON;

        -- Insert 4 maintenance records
        INSERT INTO Maintenances (Id, VehicleId, ScheduledDate, CompletedDate, Description, Cost, Type, Status)
        VALUES 
        (1, 1, '2024-01-05', '2024-01-05', 'Regular oil change and filter replacement', 85.00, 0, 2),
        (2, 1, '2024-02-20', '2024-02-21', 'Brake pad replacement and tire rotation', 320.00, 1, 2),
        (3, 1, '2024-03-15', '2024-03-15', 'Annual vehicle inspection', 50.00, 2, 2),
        (4, 1, '2024-05-01', NULL, 'Scheduled air conditioning service', 150.00, 0, 0);

        SET IDENTITY_INSERT Maintenances OFF;
        
        PRINT '✓ Inserted 4 maintenance records';
    END
    ELSE
    BEGIN
        PRINT 'ℹ Maintenance records already exist, skipping...';
    END

    -- Check if damage records already exist
    IF NOT EXISTS (SELECT 1 FROM VehicleDamages WHERE Id IN (1, 2, 3))
    BEGIN
        -- Enable identity insert for VehicleDamages
        SET IDENTITY_INSERT VehicleDamages ON;

        -- Insert 3 damage records
        INSERT INTO VehicleDamages (Id, VehicleId, RentalId, ReportedDate, Description, Severity, RepairCost, RepairedDate, ReportedBy, Status)
        VALUES 
        (1, 1, 2, '2024-02-15', 'Small scratch on rear bumper, likely from parking', 0, 150.00, '2024-02-18', 'John Doe', 2),
        (2, 1, 3, '2024-03-12', 'Dent on driver''s side door, moderate damage', 1, 450.00, '2024-03-20', 'Admin Staff', 2),
        (3, 1, NULL, '2024-04-10', 'Windshield chip from road debris', 0, 200.00, NULL, 'Employee', 1);

        SET IDENTITY_INSERT VehicleDamages OFF;
        
        PRINT '✓ Inserted 3 damage records';
    END
    ELSE
    BEGIN
        PRINT 'ℹ Damage records already exist, skipping...';
    END

    COMMIT TRANSACTION;
    
    PRINT '';
    PRINT '========================================';
    PRINT '✓ Vehicle History Data Seeded Successfully!';
    PRINT '========================================';
    PRINT '';
    PRINT 'Summary:';
    PRINT '- Vehicle ID: 1 (Toyota Corolla)';
    PRINT '- Mileage updated to: 16,000 km';
    PRINT '- Rentals: 4 completed rentals';
    PRINT '- Maintenances: 4 records (3 completed, 1 scheduled)';
    PRINT '- Damages: 3 records (2 repaired, 1 under repair)';
    PRINT '';
    PRINT 'You can now test the Vehicle History feature!';
    PRINT '';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    PRINT 'ERROR: Failed to seed data';
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR(10));
    
    THROW;
END CATCH
GO
