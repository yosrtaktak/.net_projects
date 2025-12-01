-- Create VehicleDamages table if it doesn't exist
USE [CarRentalDB];
GO

-- Check if table exists
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VehicleDamages]') AND type in (N'U'))
BEGIN
    PRINT 'Creating VehicleDamages table...';
    
    CREATE TABLE [dbo].[VehicleDamages](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [VehicleId] [int] NOT NULL,
        [RentalId] [int] NULL,
        [ReportedDate] [datetime2](7) NOT NULL,
        [Description] [nvarchar](max) NOT NULL,
        [Severity] [int] NOT NULL,
        [RepairCost] [decimal](18, 2) NOT NULL,
        [RepairedDate] [datetime2](7) NULL,
        [ReportedBy] [nvarchar](max) NULL,
        [Status] [int] NOT NULL,
        [ImageUrl] [nvarchar](max) NULL,
        CONSTRAINT [PK_VehicleDamages] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    -- Create foreign key to Vehicles
    ALTER TABLE [dbo].[VehicleDamages]  WITH CHECK ADD  
        CONSTRAINT [FK_VehicleDamages_Vehicles_VehicleId] 
        FOREIGN KEY([VehicleId])
        REFERENCES [dbo].[Vehicles] ([Id])
        ON DELETE CASCADE;
    
    -- Create foreign key to Rentals
    ALTER TABLE [dbo].[VehicleDamages]  WITH CHECK ADD  
        CONSTRAINT [FK_VehicleDamages_Rentals_RentalId] 
        FOREIGN KEY([RentalId])
        REFERENCES [dbo].[Rentals] ([Id])
        ON DELETE SET NULL;
    
    -- Create indexes
    CREATE NONCLUSTERED INDEX [IX_VehicleDamages_VehicleId] ON [dbo].[VehicleDamages]([VehicleId] ASC);
    CREATE NONCLUSTERED INDEX [IX_VehicleDamages_RentalId] ON [dbo].[VehicleDamages]([RentalId] ASC);
    
    PRINT '✓ VehicleDamages table created successfully!';
END
ELSE
BEGIN
    PRINT 'ℹ VehicleDamages table already exists.';
END
GO
