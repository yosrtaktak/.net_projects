-- Script to create Customer records for existing users without Customer records
-- This fixes the 404 error on /api/customers/me

USE CarRentalDB;
GO

-- Check for users with Customer role but no Customer record
PRINT 'Checking for users with Customer role but no Customer record...';
GO

-- Create Customer records for users who don't have one
INSERT INTO [Customers] ([FirstName], [LastName], [Email], [PhoneNumber], [DriverLicenseNumber], [DateOfBirth], [Address], [RegistrationDate], [Tier])
SELECT 
    u.[UserName] as [FirstName],
    '' as [LastName],
    u.[Email],
    '' as [PhoneNumber],
    '' as [DriverLicenseNumber],
    DATEADD(YEAR, -25, GETDATE()) as [DateOfBirth],
    NULL as [Address],
    u.[CreatedAt] as [RegistrationDate],
    0 as [Tier] -- Standard tier
FROM [AspNetUsers] u
INNER JOIN [AspNetUserRoles] ur ON u.[Id] = ur.[UserId]
INNER JOIN [AspNetRoles] r ON ur.[RoleId] = r.[Id]
LEFT JOIN [Customers] c ON u.[Email] = c.[Email]
WHERE r.[Name] = 'Customer'
AND c.[Id] IS NULL;

PRINT 'Customer records created for existing users.';
GO

-- Display results
SELECT 
    c.[Id],
    c.[FirstName],
    c.[LastName],
    c.[Email],
    c.[RegistrationDate],
    c.[Tier]
FROM [Customers] c
ORDER BY c.[RegistrationDate] DESC;
GO

PRINT 'All Customer records:';
PRINT '----------------------';
SELECT 
    COUNT(*) as TotalCustomers,
    SUM(CASE WHEN [Tier] = 0 THEN 1 ELSE 0 END) as StandardTier,
    SUM(CASE WHEN [Tier] = 1 THEN 1 ELSE 0 END) as SilverTier,
    SUM(CASE WHEN [Tier] = 2 THEN 1 ELSE 0 END) as GoldTier,
    SUM(CASE WHEN [Tier] = 3 THEN 1 ELSE 0 END) as PlatinumTier
FROM [Customers];
GO

PRINT 'Script completed successfully!';
