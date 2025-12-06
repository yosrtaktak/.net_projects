-- Add Initial Categories to Database
-- Run this script to populate the Categories table with default vehicle categories

-- First, check if categories already exist
IF NOT EXISTS (SELECT 1 FROM Categories)
BEGIN
    PRINT 'Inserting default categories...'
    
    -- Insert default categories
    INSERT INTO Categories (Name, Description, IsActive, DisplayOrder, IconUrl, CreatedAt)
    VALUES 
    ('Economy', 'Budget-friendly vehicles with excellent fuel efficiency', 1, 0, NULL, GETDATE()),
    ('Compact', 'Small, efficient cars perfect for city driving', 1, 1, NULL, GETDATE()),
    ('Midsize', 'Mid-range sedans with comfortable seating', 1, 2, NULL, GETDATE()),
    ('SUV', 'Sport Utility Vehicles with spacious interiors', 1, 3, NULL, GETDATE()),
    ('Luxury', 'Premium vehicles with high-end features', 1, 4, NULL, GETDATE()),
    ('Van', 'Large capacity vehicles for groups and families', 1, 5, NULL, GETDATE());
    
    PRINT 'Categories inserted successfully!'
    
    -- Display the inserted categories
    SELECT Id, Name, Description, DisplayOrder, IsActive, CreatedAt
    FROM Categories
    ORDER BY DisplayOrder;
END
ELSE
BEGIN
    PRINT 'Categories already exist. Skipping insertion.'
    PRINT 'Current categories:'
    
    SELECT Id, Name, Description, DisplayOrder, IsActive, CreatedAt
    FROM Categories
    ORDER BY DisplayOrder;
END

-- Verify the count
SELECT COUNT(*) as TotalCategories FROM Categories;
