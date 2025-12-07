-- ============================================================================
-- Tunisian Car Rental Database Seeding Script
-- ============================================================================
-- This script adds comprehensive Tunisian vehicle data including:
-- - Vehicles with Tunisian registration numbers
-- - Realistic Tunisian vehicle images
-- - Active rentals
-- - Vehicle damages
-- - Maintenance records
-- ============================================================================

USE [CarRentalDb]; -- Change this to your actual database name
GO

-- ============================================================================
-- SECTION 1: Categories
-- ============================================================================
PRINT 'Seeding Categories...';

-- Check and insert categories if they don't exist
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT Categories ON;
    INSERT INTO Categories (Id, Name, Description, IsActive, CreatedAt, DisplayOrder, IconUrl)
    VALUES 
        (1, 'Economy', 'Véhicules économiques pour déplacements urbains', 1, GETUTCDATE(), 1, '/images/categories/economy.png'),
        (2, 'Compact', 'Voitures compactes idéales pour la ville', 1, GETUTCDATE(), 2, '/images/categories/compact.png'),
        (3, 'Midsize', 'Voitures de taille moyenne confortables', 1, GETUTCDATE(), 3, '/images/categories/midsize.png'),
        (4, 'SUV', 'SUV spacieux pour familles et voyages', 1, GETUTCDATE(), 4, '/images/categories/suv.png'),
        (5, 'Luxury', 'Véhicules de luxe haut de gamme', 1, GETUTCDATE(), 5, '/images/categories/luxury.png'),
        (6, 'Van', 'Minibus et utilitaires pour groupes', 1, GETUTCDATE(), 6, '/images/categories/van.png'),
        (7, 'Sports', 'Voitures de sport haute performance', 1, GETUTCDATE(), 7, '/images/categories/sports.png');
    SET IDENTITY_INSERT Categories OFF;
    PRINT 'Categories inserted successfully.';
END
ELSE
BEGIN
    PRINT 'Categories already exist.';
END
GO

-- ============================================================================
-- SECTION 2: Tunisian Vehicles
-- ============================================================================
PRINT 'Seeding Tunisian Vehicles...';

-- Economy Vehicles
INSERT INTO Vehicles (Brand, Model, RegistrationNumber, Year, CategoryId, DailyRate, Status, Mileage, FuelType, SeatingCapacity, ImageUrl)
VALUES 
    ('Fiat', 'Punto', '147 TU 3298', 2020, 1, 45.00, 0, 45000, 'Essence', 5, 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800'),
    ('Renault', 'Clio V', '238 TU 5612', 2021, 1, 50.00, 1, 32000, 'Diesel', 5, 'https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800'),
    ('Peugeot', '208', '512 TU 7845', 2022, 1, 55.00, 0, 18000, 'Essence', 5, 'https://images.unsplash.com/photo-1583267746897-0a11b0a2b94e?w=800'),
    ('Dacia', 'Sandero', '694 TU 2134', 2020, 1, 42.00, 0, 52000, 'Diesel', 5, 'https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?w=800'),
    ('Kia', 'Picanto', '823 TU 9871', 2021, 1, 48.00, 2, 28000, 'Essence', 5, 'https://images.unsplash.com/photo-1568605117036-5fe5e7bab0b7?w=800'),
    ('Hyundai', 'i10', '156 TU 4523', 2022, 1, 47.00, 0, 15000, 'Essence', 5, 'https://images.unsplash.com/photo-1617531653332-bd46c24f2068?w=800'),
    
    -- Compact Vehicles
    ('Toyota', 'Corolla', '234 TU 1567', 2021, 2, 65.00, 1, 38000, 'Hybrid', 5, 'https://images.unsplash.com/photo-1623869675781-80aa31bfa4e8?w=800'),
    ('Volkswagen', 'Golf 8', '389 TU 8942', 2022, 2, 70.00, 0, 22000, 'Diesel', 5, 'https://images.unsplash.com/photo-1622353219448-46a009f0d44f?w=800'),
    ('Seat', 'Leon', '567 TU 3214', 2021, 2, 62.00, 0, 35000, 'Essence', 5, 'https://images.unsplash.com/photo-1590362891991-f776e747a588?w=800'),
    ('Hyundai', 'i30', '741 TU 6789', 2020, 2, 60.00, 3, 48000, 'Diesel', 5, 'https://images.unsplash.com/photo-1619682817481-e994891cd1f5?w=800'),
    ('Opel', 'Astra', '892 TU 4125', 2021, 2, 63.00, 0, 31000, 'Essence', 5, 'https://images.unsplash.com/photo-1583267746897-0a11b0a2b94e?w=800'),
    
    -- Midsize Vehicles
    ('Honda', 'Accord', '123 TU 7856', 2022, 3, 80.00, 1, 25000, 'Hybrid', 5, 'https://images.unsplash.com/photo-1590362891991-f776e747a588?w=800'),
    ('Mazda', '6', '456 TU 2341', 2021, 3, 75.00, 0, 33000, 'Essence', 5, 'https://images.unsplash.com/photo-1617531653332-bd46c24f2068?w=800'),
    ('Peugeot', '508', '678 TU 9012', 2022, 3, 85.00, 0, 19000, 'Diesel', 5, 'https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?w=800'),
    ('Ford', 'Mondeo', '789 TU 5634', 2020, 3, 72.00, 2, 42000, 'Diesel', 5, 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800'),
    ('Nissan', 'Altima', '234 TU 8901', 2021, 3, 78.00, 0, 29000, 'Essence', 5, 'https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800'),
    
    -- SUV Vehicles
    ('BMW', 'X3', '345 TU 1234', 2022, 4, 120.00, 0, 18000, 'Diesel', 5, 'https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800'),
    ('Toyota', 'RAV4', '456 TU 5678', 2021, 4, 95.00, 1, 35000, 'Hybrid', 5, 'https://images.unsplash.com/photo-1609521263047-f8f205293f24?w=800'),
    ('Nissan', 'Qashqai', '567 TU 9012', 2022, 4, 90.00, 0, 21000, 'Essence', 5, 'https://images.unsplash.com/photo-1602230833460-8f29eb0d29c8?w=800'),
    ('Hyundai', 'Tucson', '678 TU 3456', 2021, 4, 88.00, 1, 32000, 'Diesel', 5, 'https://images.unsplash.com/photo-1611651338412-8403fa6e3599?w=800'),
    ('Kia', 'Sportage', '789 TU 7890', 2022, 4, 92.00, 0, 24000, 'Diesel', 5, 'https://images.unsplash.com/photo-1617531653332-bd46c24f2068?w=800'),
    ('Peugeot', '3008', '890 TU 1234', 2021, 4, 85.00, 2, 38000, 'Diesel', 5, 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800'),
    ('Renault', 'Kadjar', '912 TU 5678', 2020, 4, 82.00, 0, 45000, 'Diesel', 5, 'https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800'),
    
    -- Luxury Vehicles
    ('Mercedes-Benz', 'Classe E', '111 TU 2222', 2023, 5, 180.00, 0, 8000, 'Diesel', 5, 'https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=800'),
    ('BMW', 'Série 5', '222 TU 3333', 2022, 5, 175.00, 1, 15000, 'Hybrid', 5, 'https://images.unsplash.com/photo-1555215695-3004980ad54e?w=800'),
    ('Audi', 'A6', '333 TU 4444', 2023, 5, 170.00, 0, 10000, 'Diesel', 5, 'https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=800'),
    ('Lexus', 'ES 300h', '444 TU 5555', 2022, 5, 165.00, 0, 12000, 'Hybrid', 5, 'https://images.unsplash.com/photo-1621993202323-f438eec934ff?w=800'),
    ('Mercedes-Benz', 'Classe C', '555 TU 6666', 2021, 5, 155.00, 3, 25000, 'Essence', 5, 'https://images.unsplash.com/photo-1617531653332-bd46c24f2068?w=800'),
    
    -- Van/Minibus
    ('Volkswagen', 'Transporter', '666 TU 7777', 2021, 6, 110.00, 0, 38000, 'Diesel', 9, 'https://images.unsplash.com/photo-1527786356703-4b100091cd2c?w=800'),
    ('Mercedes-Benz', 'Vito', '777 TU 8888', 2022, 6, 125.00, 1, 22000, 'Diesel', 8, 'https://images.unsplash.com/photo-1544620347-c4fd4a3d5957?w=800'),
    ('Renault', 'Trafic', '888 TU 9999', 2021, 6, 105.00, 0, 35000, 'Diesel', 9, 'https://images.unsplash.com/photo-1562519819-019d3b5c6a55?w=800'),
    ('Peugeot', 'Traveller', '999 TU 1111', 2022, 6, 115.00, 2, 28000, 'Diesel', 8, 'https://images.unsplash.com/photo-1527786356703-4b100091cd2c?w=800'),
    
    -- Sports Cars
    ('Porsche', '718 Cayman', '101 TU 2023', 2023, 7, 350.00, 0, 5000, 'Essence', 2, 'https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=800'),
    ('BMW', 'M4', '202 TU 2023', 2022, 7, 320.00, 0, 8000, 'Essence', 4, 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800'),
    ('Audi', 'RS5', '303 TU 2023', 2023, 7, 330.00, 1, 6000, 'Essence', 4, 'https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=800'),
    ('Mercedes-AMG', 'C63', '404 TU 2023', 2022, 7, 340.00, 0, 9000, 'Essence', 4, 'https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=800'),
    ('Nissan', 'GT-R', '505 TU 2023', 2021, 7, 400.00, 0, 12000, 'Essence', 4, 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=800');

PRINT 'Tunisian Vehicles inserted successfully.';
GO

-- ============================================================================
-- SECTION 3: Active Rentals (Requires AspNetUsers)
-- ============================================================================
PRINT 'Seeding Active Rentals...';

-- Note: Replace the UserId values with actual user IDs from your AspNetUsers table
-- You can run: SELECT Id, Email FROM AspNetUsers; to get the actual IDs

DECLARE @UserId1 NVARCHAR(450) = (SELECT TOP 1 Id FROM AspNetUsers WHERE Email LIKE '%@%' ORDER BY Id);
DECLARE @UserId2 NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email LIKE '%@%' ORDER BY Id OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @UserId3 NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email LIKE '%@%' ORDER BY Id OFFSET 2 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @UserId4 NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email LIKE '%@%' ORDER BY Id OFFSET 3 ROWS FETCH NEXT 1 ROWS ONLY);
DECLARE @UserId5 NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email LIKE '%@%' ORDER BY Id OFFSET 4 ROWS FETCH NEXT 1 ROWS ONLY);

-- Active Rentals (Status = 1 for Active)
IF @UserId1 IS NOT NULL
BEGIN
    INSERT INTO Rentals (UserId, VehicleId, StartDate, EndDate, ActualReturnDate, TotalCost, Status, StartMileage, EndMileage, Notes, CreatedAt)
    VALUES 
        -- Active Rentals
        (@UserId1, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '238 TU 5612'), DATEADD(day, -5, GETDATE()), DATEADD(day, 2, GETDATE()), NULL, 350.00, 1, 32000, NULL, 'Location active - Client à Tunis', DATEADD(day, -6, GETDATE())),
        (@UserId2, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '234 TU 1567'), DATEADD(day, -3, GETDATE()), DATEADD(day, 4, GETDATE()), NULL, 455.00, 1, 38000, NULL, 'Location active - Voyage Hammamet', DATEADD(day, -4, GETDATE())),
        (@UserId3, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '123 TU 7856'), DATEADD(day, -7, GETDATE()), DATEADD(day, 3, GETDATE()), NULL, 800.00, 1, 25000, NULL, 'Location active - Voyage d''affaires Sfax', DATEADD(day, -8, GETDATE())),
        (@UserId4, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '456 TU 5678'), DATEADD(day, -2, GETDATE()), DATEADD(day, 5, GETDATE()), NULL, 665.00, 1, 35000, NULL, 'Location active - Vacances famille Sousse', DATEADD(day, -3, GETDATE())),
        (@UserId5, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '678 TU 3456'), DATEADD(day, -4, GETDATE()), DATEADD(day, 3, GETDATE()), NULL, 616.00, 1, 32000, NULL, 'Location active - Visite Djerba', DATEADD(day, -5, GETDATE())),
        
        -- Reserved Rentals (Status = 0 for Reserved)
        (@UserId1, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '823 TU 9871'), DATEADD(day, 2, GETDATE()), DATEADD(day, 9, GETDATE()), NULL, 336.00, 0, NULL, NULL, 'Réservation - Récupération à l''aéroport Tunis-Carthage', GETDATE()),
        (@UserId2, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '222 TU 3333'), DATEADD(day, 5, GETDATE()), DATEADD(day, 8, GETDATE()), NULL, 525.00, 0, NULL, NULL, 'Réservation - Événement spécial Sidi Bou Said', GETDATE()),
        (@UserId3, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '303 TU 2023'), DATEADD(day, 7, GETDATE()), DATEADD(day, 10, GETDATE()), NULL, 990.00, 0, NULL, NULL, 'Réservation - Week-end luxe Hammamet', GETDATE()),
        
        -- Completed Rentals (Status = 2 for Completed)
        (@UserId4, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '147 TU 3298'), DATEADD(day, -20, GETDATE()), DATEADD(day, -15, GETDATE()), DATEADD(day, -15, GETDATE()), 225.00, 2, 44500, 45000, 'Location terminée - Aucun problème', DATEADD(day, -21, GETDATE())),
        (@UserId5, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '512 TU 7845'), DATEADD(day, -18, GETDATE()), DATEADD(day, -14, GETDATE()), DATEADD(day, -14, GETDATE()), 220.00, 2, 17600, 18000, 'Location terminée - Retour à temps', DATEADD(day, -19, GETDATE())),
        (@UserId1, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '789 TU 5634'), DATEADD(day, -25, GETDATE()), DATEADD(day, -18, GETDATE()), DATEADD(day, -18, GETDATE()), 504.00, 2, 41500, 42000, 'Location terminée - Voyage Monastir', DATEADD(day, -26, GETDATE())),
        (@UserId2, (SELECT Id FROM Vehicles WHERE RegistrationNumber = '555 TU 6666'), DATEADD(day, -30, GETDATE()), DATEADD(day, -25, GETDATE()), DATEADD(day, -24, GETDATE()), 775.00, 2, 24500, 25000, 'Location terminée - Retour anticipé', DATEADD(day, -31, GETDATE()));
    
    PRINT 'Rentals inserted successfully.';
END
ELSE
BEGIN
    PRINT 'WARNING: No users found. Please create users first before running rental seeding.';
END
GO

-- ============================================================================
-- SECTION 4: Vehicle Damages
-- ============================================================================
PRINT 'Seeding Vehicle Damages...';

-- Damages on various vehicles
INSERT INTO VehicleDamages (VehicleId, RentalId, ReportedDate, Description, Severity, RepairCost, RepairedDate, ReportedBy, ImageUrl, Status)
VALUES 
    -- Minor damage on returned vehicle
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '789 TU 5634'),
     (SELECT TOP 1 Id FROM Rentals WHERE VehicleId = (SELECT Id FROM Vehicles WHERE RegistrationNumber = '789 TU 5634')),
     DATEADD(day, -18, GETDATE()),
     'Rayure superficielle sur la porte arrière gauche, probablement causée lors du stationnement. Nécessite polissage et retouche de peinture.',
     0, -- Minor
     180.00,
     DATEADD(day, -15, GETDATE()),
     'Inspection de retour',
     NULL,
     2), -- Repaired
    
    -- Moderate damage requiring repair
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '741 TU 6789'),
     NULL,
     DATEADD(day, -10, GETDATE()),
     'Phare avant droit cassé et pare-chocs avant endommagé. Impact important nécessitant remplacement des pièces. Véhicule en maintenance.',
     1, -- Moderate
     850.00,
     NULL,
     'Client: Ahmed Ben Salah',
     NULL,
     1), -- UnderRepair
    
    -- Major damage on vehicle
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '890 TU 1234'),
     NULL,
     DATEADD(day, -15, GETDATE()),
     'Dommages importants au système de suspension avant après passage sur nid-de-poule. Pneu avant gauche crevé. Amortisseur et triangle de suspension à remplacer.',
     2, -- Major
     1250.00,
     NULL,
     'Garage Technique Auto Sfax',
     NULL,
     1), -- UnderRepair
    
    -- Minor damage recently reported
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '823 TU 9871'),
     NULL,
     DATEADD(day, -5, GETDATE()),
     'Rétroviseur extérieur droit légèrement endommagé. Boîtier fissuré mais fonctionnel. Réparation planifiée.',
     0, -- Minor
     120.00,
     NULL,
     'Inspection routine',
     NULL,
     0), -- Reported
    
    -- Critical damage on luxury vehicle
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '555 TU 6666'),
     (SELECT TOP 1 Id FROM Rentals WHERE VehicleId = (SELECT Id FROM Vehicles WHERE RegistrationNumber = '555 TU 6666') AND Status = 2),
     DATEADD(day, -24, GETDATE()),
     'Dommages critiques à l''aile arrière droite suite à un accrochage. Carrosserie déformée, peinture écaillée, feu arrière cassé. Réparation en carrosserie spécialisée Mercedes.',
     3, -- Critical
     2800.00,
     DATEADD(day, -2, GETDATE()),
     'Centre de Carrosserie Mercedes Tunis',
     NULL,
     2); -- Repaired

PRINT 'Vehicle Damages inserted successfully.';
GO

-- ============================================================================
-- SECTION 5: Maintenance Records
-- ============================================================================
PRINT 'Seeding Maintenance Records...';

-- Maintenance for various vehicles
INSERT INTO Maintenances (VehicleId, ScheduledDate, CompletedDate, Description, Cost, Type, Status)
VALUES 
    -- Routine Maintenance (Completed)
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '147 TU 3298'), DATEADD(day, -30, GETDATE()), DATEADD(day, -30, GETDATE()), 'Vidange moteur et remplacement du filtre à huile. Contrôle des niveaux.', 95.00, 0, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '238 TU 5612'), DATEADD(day, -25, GETDATE()), DATEADD(day, -25, GETDATE()), 'Entretien périodique 30 000 km - Vidange, filtres, bougies', 285.00, 0, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '512 TU 7845'), DATEADD(day, -20, GETDATE()), DATEADD(day, -20, GETDATE()), 'Contrôle technique annuel et révision complète', 350.00, 2, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '234 TU 1567'), DATEADD(day, -15, GETDATE()), DATEADD(day, -15, GETDATE()), 'Remplacement plaquettes de frein avant et disques', 420.00, 1, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '389 TU 8942'), DATEADD(day, -35, GETDATE()), DATEADD(day, -35, GETDATE()), 'Vidange boîte automatique et changement filtre habitacle', 180.00, 0, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '567 TU 3214'), DATEADD(day, -40, GETDATE()), DATEADD(day, -40, GETDATE()), 'Permutation et équilibrage des pneus', 75.00, 0, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '123 TU 7856'), DATEADD(day, -28, GETDATE()), DATEADD(day, -28, GETDATE()), 'Entretien système hybride - Contrôle batterie', 220.00, 2, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '456 TU 2341'), DATEADD(day, -45, GETDATE()), DATEADD(day, -45, GETDATE()), 'Remplacement courroie de distribution', 680.00, 1, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '678 TU 9012'), DATEADD(day, -22, GETDATE()), DATEADD(day, -22, GETDATE()), 'Vidange et nettoyage injecteurs diesel', 240.00, 0, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '345 TU 1234'), DATEADD(day, -18, GETDATE()), DATEADD(day, -18, GETDATE()), 'Révision BMW 20 000 km - Service complet', 550.00, 0, 2),
    
    -- Repairs (Completed)
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '456 TU 5678'), DATEADD(day, -50, GETDATE()), DATEADD(day, -48, GETDATE()), 'Réparation système de climatisation - Recharge gaz R134a', 180.00, 1, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '567 TU 9012'), DATEADD(day, -42, GETDATE()), DATEADD(day, -41, GETDATE()), 'Remplacement batterie 12V', 145.00, 1, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '789 TU 7890'), DATEADD(day, -38, GETDATE()), DATEADD(day, -37, GETDATE()), 'Réparation capteur de stationnement arrière', 95.00, 1, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '111 TU 2222'), DATEADD(day, -33, GETDATE()), DATEADD(day, -32, GETDATE()), 'Recalibrage système ADAS Mercedes', 280.00, 1, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '666 TU 7777'), DATEADD(day, -27, GETDATE()), DATEADD(day, -26, GETDATE()), 'Remplacement alternateur', 420.00, 1, 2),
    
    -- In Progress Maintenance
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '741 TU 6789'), DATEADD(day, -10, GETDATE()), NULL, 'Réparation phare avant et pare-chocs - En cours chez carrossier', 850.00, 1, 1),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '890 TU 1234'), DATEADD(day, -15, GETDATE()), NULL, 'Remplacement suspension avant complète - Pièces commandées', 1250.00, 1, 1),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '999 TU 1111'), DATEADD(day, -8, GETDATE()), NULL, 'Révision moteur diesel - Diagnostic en cours', 450.00, 2, 1),
    
    -- Scheduled Future Maintenance
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '147 TU 3298'), DATEADD(day, 5, GETDATE()), NULL, 'Vidange programmée 45 000 km', 95.00, 0, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '694 TU 2134'), DATEADD(day, 10, GETDATE()), NULL, 'Révision périodique 50 000 km', 320.00, 0, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '156 TU 4523'), DATEADD(day, 7, GETDATE()), NULL, 'Contrôle et remplacement pneus si nécessaire', 280.00, 2, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '892 TU 4125'), DATEADD(day, 15, GETDATE()), NULL, 'Contrôle technique annuel obligatoire', 60.00, 2, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '234 TU 8901'), DATEADD(day, 12, GETDATE()), NULL, 'Entretien climatisation avant été', 120.00, 0, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '912 TU 5678'), DATEADD(day, 20, GETDATE()), NULL, 'Remplacement filtre à air et filtre habitacle', 85.00, 0, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '333 TU 4444'), DATEADD(day, 8, GETDATE()), NULL, 'Révision Audi 10 000 km - Service premium', 480.00, 0, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '444 TU 5555'), DATEADD(day, 18, GETDATE()), NULL, 'Contrôle système hybride Lexus', 250.00, 2, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '777 TU 8888'), DATEADD(day, 14, GETDATE()), NULL, 'Révision Mercedes Vito 20 000 km', 380.00, 0, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '888 TU 9999'), DATEADD(day, 22, GETDATE()), NULL, 'Entretien Renault Trafic - Vidange + filtres', 195.00, 0, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '101 TU 2023'), DATEADD(day, 25, GETDATE()), NULL, 'Révision Porsche 5 000 km - Centre agréé', 750.00, 0, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '202 TU 2023'), DATEADD(day, 16, GETDATE()), NULL, 'Contrôle BMW M4 - Performance check', 420.00, 2, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '404 TU 2023'), DATEADD(day, 19, GETDATE()), NULL, 'Révision AMG - Service sportif', 680.00, 0, 0),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '505 TU 2023'), DATEADD(day, 11, GETDATE()), NULL, 'Entretien Nissan GT-R - Contrôle turbo', 550.00, 2, 0),
    
    -- Emergency Maintenance (Past)
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '678 TU 3456'), DATEADD(day, -12, GETDATE()), DATEADD(day, -12, GETDATE()), 'Crevaison pneu avant - Réparation urgente', 45.00, 3, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '222 TU 3333'), DATEADD(day, -8, GETDATE()), DATEADD(day, -8, GETDATE()), 'Problème électrique - Fusible tableau de bord', 35.00, 3, 2),
    ((SELECT Id FROM Vehicles WHERE RegistrationNumber = '303 TU 2023'), DATEADD(day, -5, GETDATE()), DATEADD(day, -5, GETDATE()), 'Surchauffe moteur - Ajout liquide de refroidissement', 65.00, 3, 2);

PRINT 'Maintenance Records inserted successfully.';
GO

-- ============================================================================
-- SECTION 6: Update Statistics and Summary
-- ============================================================================
PRINT '';
PRINT '============================================================================';
PRINT 'SEEDING COMPLETED SUCCESSFULLY';
PRINT '============================================================================';
PRINT '';
PRINT 'Database Summary:';
PRINT '-----------------';

DECLARE @CategoryCount INT = (SELECT COUNT(*) FROM Categories);
DECLARE @VehicleCount INT = (SELECT COUNT(*) FROM Vehicles);
DECLARE @RentalCount INT = (SELECT COUNT(*) FROM Rentals);
DECLARE @DamageCount INT = (SELECT COUNT(*) FROM VehicleDamages);
DECLARE @MaintenanceCount INT = (SELECT COUNT(*) FROM Maintenances);

PRINT 'Categories: ' + CAST(@CategoryCount AS VARCHAR(10));
PRINT 'Vehicles: ' + CAST(@VehicleCount AS VARCHAR(10));
PRINT 'Rentals: ' + CAST(@RentalCount AS VARCHAR(10));
PRINT 'Vehicle Damages: ' + CAST(@DamageCount AS VARCHAR(10));
PRINT 'Maintenance Records: ' + CAST(@MaintenanceCount AS VARCHAR(10));
PRINT '';

-- Vehicle Status Breakdown
PRINT 'Vehicle Status Breakdown:';
PRINT '-------------------------';
PRINT 'Available: ' + CAST((SELECT COUNT(*) FROM Vehicles WHERE Status = 0) AS VARCHAR(10));
PRINT 'Rented: ' + CAST((SELECT COUNT(*) FROM Vehicles WHERE Status = 1) AS VARCHAR(10));
PRINT 'Reserved: ' + CAST((SELECT COUNT(*) FROM Vehicles WHERE Status = 2) AS VARCHAR(10));
PRINT 'Maintenance: ' + CAST((SELECT COUNT(*) FROM Vehicles WHERE Status = 3) AS VARCHAR(10));
PRINT '';

-- Rental Status Breakdown
IF @RentalCount > 0
BEGIN
    PRINT 'Rental Status Breakdown:';
    PRINT '------------------------';
    PRINT 'Reserved: ' + CAST((SELECT COUNT(*) FROM Rentals WHERE Status = 0) AS VARCHAR(10));
    PRINT 'Active: ' + CAST((SELECT COUNT(*) FROM Rentals WHERE Status = 1) AS VARCHAR(10));
    PRINT 'Completed: ' + CAST((SELECT COUNT(*) FROM Rentals WHERE Status = 2) AS VARCHAR(10));
    PRINT '';
END

PRINT 'Maintenance Status Breakdown:';
PRINT '-----------------------------';
PRINT 'Scheduled: ' + CAST((SELECT COUNT(*) FROM Maintenances WHERE Status = 0) AS VARCHAR(10));
PRINT 'In Progress: ' + CAST((SELECT COUNT(*) FROM Maintenances WHERE Status = 1) AS VARCHAR(10));
PRINT 'Completed: ' + CAST((SELECT COUNT(*) FROM Maintenances WHERE Status = 2) AS VARCHAR(10));
PRINT '';

PRINT 'Damage Status Breakdown:';
PRINT '------------------------';
PRINT 'Reported: ' + CAST((SELECT COUNT(*) FROM VehicleDamages WHERE Status = 0) AS VARCHAR(10));
PRINT 'Under Repair: ' + CAST((SELECT COUNT(*) FROM VehicleDamages WHERE Status = 1) AS VARCHAR(10));
PRINT 'Repaired: ' + CAST((SELECT COUNT(*) FROM VehicleDamages WHERE Status = 2) AS VARCHAR(10));
PRINT '';

PRINT '============================================================================';
PRINT 'All data has been seeded successfully!';
PRINT 'You can now test the application with realistic Tunisian data.';
PRINT '============================================================================';
GO
