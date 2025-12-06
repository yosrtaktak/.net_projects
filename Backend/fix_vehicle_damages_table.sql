-- Script to create VehicleDamages table and seed data
-- This fixes the issue where migrations were recorded but didn't actually create the table

USE CarRentalDB;
GO

-- Create VehicleDamages table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'VehicleDamages')
BEGIN
    CREATE TABLE [VehicleDamages] (
        [Id] int NOT NULL IDENTITY,
        [VehicleId] int NOT NULL,
        [RentalId] int NULL,
        [ReportedDate] datetime2 NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Severity] int NOT NULL,
        [RepairCost] decimal(18,2) NOT NULL,
        [RepairedDate] datetime2 NULL,
        [ReportedBy] nvarchar(max) NULL,
        [ImageUrl] nvarchar(max) NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_VehicleDamages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VehicleDamages_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_VehicleDamages_Rentals_RentalId] FOREIGN KEY ([RentalId]) REFERENCES [Rentals] ([Id]) ON DELETE SET NULL
    );

    CREATE INDEX [IX_VehicleDamages_VehicleId] ON [VehicleDamages] ([VehicleId]);
    CREATE INDEX [IX_VehicleDamages_RentalId] ON [VehicleDamages] ([RentalId]);
    
    PRINT 'VehicleDamages table created successfully';
END
ELSE
BEGIN
    PRINT 'VehicleDamages table already exists';
END
GO

-- Update Vehicle mileage (from model snapshot)
UPDATE [Vehicles] SET [Mileage] = 16000 WHERE [Id] = 1;
GO

-- Insert Maintenance seed data if not exists
IF NOT EXISTS (SELECT 1 FROM [Maintenances])
BEGIN
    SET IDENTITY_INSERT [Maintenances] ON;
    
    INSERT INTO [Maintenances] ([Id], [VehicleId], [ScheduledDate], [CompletedDate], [Description], [Cost], [Type], [Status])
    VALUES 
        (1, 1, '2024-01-05', '2024-01-05', 'Regular oil change and filter replacement', 85.00, 0, 2),
        (2, 1, '2024-02-20', '2024-02-21', 'Brake pad replacement and tire rotation', 320.00, 1, 2),
        (3, 1, '2024-03-15', '2024-03-15', 'Annual vehicle inspection', 50.00, 2, 2),
        (4, 1, '2024-05-01', NULL, 'Scheduled air conditioning service', 150.00, 0, 0);
    
    SET IDENTITY_INSERT [Maintenances] OFF;
    PRINT 'Maintenance seed data inserted';
END
GO

-- Insert Rental seed data if not exists
IF NOT EXISTS (SELECT 1 FROM [Rentals])
BEGIN
    SET IDENTITY_INSERT [Rentals] ON;
    
    INSERT INTO [Rentals] ([Id], [CustomerId], [VehicleId], [StartDate], [EndDate], [ActualReturnDate], [TotalCost], [Status], [StartMileage], [EndMileage], [Notes], [CreatedAt])
    VALUES 
        (1, 1, 1, '2024-01-15', '2024-01-20', '2024-01-20', 175.00, 2, 15000, 15250, 'Regular rental, vehicle returned in good condition', '2024-01-10'),
        (2, 1, 1, '2024-02-10', '2024-02-15', '2024-02-15', 175.00, 2, 15250, 15480, 'Weekend trip rental', '2024-02-05'),
        (3, 1, 1, '2024-03-05', '2024-03-12', '2024-03-12', 245.00, 2, 15480, 15820, 'Business trip rental', '2024-03-01'),
        (4, 1, 1, '2024-04-01', '2024-04-05', '2024-04-06', 175.00, 2, 15820, 16000, 'Returned 1 day late, extra charges applied', '2024-03-28');
    
    SET IDENTITY_INSERT [Rentals] OFF;
    PRINT 'Rental seed data inserted';
END
GO

-- Insert VehicleDamages seed data if not exists
IF NOT EXISTS (SELECT 1 FROM [VehicleDamages])
BEGIN
    SET IDENTITY_INSERT [VehicleDamages] ON;
    
    INSERT INTO [VehicleDamages] ([Id], [VehicleId], [RentalId], [ReportedDate], [Description], [Severity], [RepairCost], [RepairedDate], [ReportedBy], [ImageUrl], [Status])
    VALUES 
        (1, 1, 2, '2024-02-15', 'Small scratch on rear bumper, likely from parking', 0, 150.00, '2024-02-18', 'John Doe', NULL, 2),
        (2, 1, 3, '2024-03-12', 'Dent on driver''s side door, moderate damage', 1, 450.00, '2024-03-20', 'Admin Staff', NULL, 2),
        (3, 1, NULL, '2024-04-10', 'Windshield chip from road debris', 0, 200.00, NULL, 'Employee', NULL, 1);
    
    SET IDENTITY_INSERT [VehicleDamages] OFF;
    PRINT 'VehicleDamages seed data inserted';
END
GO

PRINT 'Database sync completed successfully!';
