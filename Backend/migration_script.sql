IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FirstName] nvarchar(max) NULL,
        [LastName] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [LastLogin] datetime2 NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [Customers] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(450) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [DriverLicenseNumber] nvarchar(450) NOT NULL,
        [DateOfBirth] datetime2 NOT NULL,
        [Address] nvarchar(max) NULL,
        [RegistrationDate] datetime2 NOT NULL,
        [Tier] int NOT NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [Vehicles] (
        [Id] int NOT NULL IDENTITY,
        [Brand] nvarchar(100) NOT NULL,
        [Model] nvarchar(100) NOT NULL,
        [RegistrationNumber] nvarchar(50) NOT NULL,
        [Year] int NOT NULL,
        [Category] int NOT NULL,
        [DailyRate] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [ImageUrl] nvarchar(max) NULL,
        [Mileage] int NOT NULL,
        [FuelType] nvarchar(max) NULL,
        [SeatingCapacity] int NOT NULL,
        CONSTRAINT [PK_Vehicles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [Maintenances] (
        [Id] int NOT NULL IDENTITY,
        [VehicleId] int NOT NULL,
        [ScheduledDate] datetime2 NOT NULL,
        [CompletedDate] datetime2 NULL,
        [Description] nvarchar(max) NOT NULL,
        [Cost] decimal(18,2) NOT NULL,
        [Type] int NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Maintenances] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Maintenances_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [Rentals] (
        [Id] int NOT NULL IDENTITY,
        [CustomerId] int NOT NULL,
        [VehicleId] int NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [ActualReturnDate] datetime2 NULL,
        [TotalCost] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [StartMileage] int NULL,
        [EndMileage] int NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Rentals] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Rentals_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Rentals_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] int NOT NULL IDENTITY,
        [RentalId] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Method] int NOT NULL,
        [Status] int NOT NULL,
        [PaymentDate] datetime2 NOT NULL,
        [TransactionId] nvarchar(max) NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_Rentals_RentalId] FOREIGN KEY ([RentalId]) REFERENCES [Rentals] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'DateOfBirth', N'DriverLicenseNumber', N'Email', N'FirstName', N'LastName', N'PhoneNumber', N'RegistrationDate', N'Tier') AND [object_id] = OBJECT_ID(N'[Customers]'))
        SET IDENTITY_INSERT [Customers] ON;
    EXEC(N'INSERT INTO [Customers] ([Id], [Address], [DateOfBirth], [DriverLicenseNumber], [Email], [FirstName], [LastName], [PhoneNumber], [RegistrationDate], [Tier])
    VALUES (1, N''123 Main St, City, Country'', ''1990-05-15T00:00:00.0000000'', N''DL123456'', N''john.doe@example.com'', N''John'', N''Doe'', N''+1234567890'', ''2024-01-01T00:00:00.0000000'', 2)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'DateOfBirth', N'DriverLicenseNumber', N'Email', N'FirstName', N'LastName', N'PhoneNumber', N'RegistrationDate', N'Tier') AND [object_id] = OBJECT_ID(N'[Customers]'))
        SET IDENTITY_INSERT [Customers] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Brand', N'Category', N'DailyRate', N'FuelType', N'ImageUrl', N'Mileage', N'Model', N'RegistrationNumber', N'SeatingCapacity', N'Status', N'Year') AND [object_id] = OBJECT_ID(N'[Vehicles]'))
        SET IDENTITY_INSERT [Vehicles] ON;
    EXEC(N'INSERT INTO [Vehicles] ([Id], [Brand], [Category], [DailyRate], [FuelType], [ImageUrl], [Mileage], [Model], [RegistrationNumber], [SeatingCapacity], [Status], [Year])
    VALUES (1, N''Toyota'', 1, 35.0, N''Gasoline'', NULL, 15000, N''Corolla'', N''ABC123'', 5, 0, 2023),
    (2, N''BMW'', 3, 85.0, N''Diesel'', NULL, 5000, N''X5'', N''XYZ789'', 7, 0, 2024),
    (3, N''Mercedes-Benz'', 4, 120.0, N''Gasoline'', NULL, 3000, N''C-Class'', N''LUX456'', 5, 0, 2024),
    (4, N''Honda'', 0, 28.0, N''Gasoline'', NULL, 20000, N''Civic'', N''ECO789'', 5, 0, 2023)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Brand', N'Category', N'DailyRate', N'FuelType', N'ImageUrl', N'Mileage', N'Model', N'RegistrationNumber', N'SeatingCapacity', N'Status', N'Year') AND [object_id] = OBJECT_ID(N'[Vehicles]'))
        SET IDENTITY_INSERT [Vehicles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Customers_DriverLicenseNumber] ON [Customers] ([DriverLicenseNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Customers_Email] ON [Customers] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Maintenances_VehicleId] ON [Maintenances] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Payments_RentalId] ON [Payments] ([RentalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Rentals_CustomerId] ON [Rentals] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Rentals_VehicleId] ON [Rentals] ([VehicleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vehicles_RegistrationNumber] ON [Vehicles] ([RegistrationNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251121193118_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251121193118_InitialCreate', N'9.0.0');
END;

COMMIT;
GO

