using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class MigrateToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only drop if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_Customers_CustomerId')
                BEGIN
                    ALTER TABLE [Rentals] DROP CONSTRAINT [FK_Rentals_Customers_CustomerId];
                END
            ");

            // Only drop Customers table if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
                BEGIN
                    DROP TABLE [Customers];
                END
            ");

            // Only drop index if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_CustomerId' AND object_id = OBJECT_ID('Rentals'))
                BEGIN
                    DROP INDEX [IX_Rentals_CustomerId] ON [Rentals];
                END
            ");

            // Only drop CustomerId column if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'CustomerId')
                BEGIN
                    DECLARE @var0 sysname;
                    SELECT @var0 = [d].[name]
                    FROM [sys].[default_constraints] [d]
                    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rentals]') AND [c].[name] = N'CustomerId');
                    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Rentals] DROP CONSTRAINT [' + @var0 + '];');
                    ALTER TABLE [Rentals] DROP COLUMN [CustomerId];
                END
            ");

            // Only add UserId if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Rentals') AND name = 'UserId')
                BEGIN
                    ALTER TABLE [Rentals] ADD [UserId] nvarchar(450) NOT NULL DEFAULT N'';
                END
            ");

            // Only add AspNetUsers columns if not exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Address')
                BEGIN
                    ALTER TABLE [AspNetUsers] ADD [Address] nvarchar(500) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DateOfBirth')
                BEGIN
                    ALTER TABLE [AspNetUsers] ADD [DateOfBirth] datetime2 NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'DriverLicenseNumber')
                BEGIN
                    ALTER TABLE [AspNetUsers] ADD [DriverLicenseNumber] nvarchar(50) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RegistrationDate')
                BEGIN
                    ALTER TABLE [AspNetUsers] ADD [RegistrationDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Tier')
                BEGIN
                    ALTER TABLE [AspNetUsers] ADD [Tier] int NOT NULL DEFAULT 0;
                END
            ");

            // Only create VehicleDamages if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VehicleDamages')
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
                        CONSTRAINT [FK_VehicleDamages_Rentals_RentalId] FOREIGN KEY ([RentalId]) REFERENCES [Rentals] ([Id]) ON DELETE SET NULL,
                        CONSTRAINT [FK_VehicleDamages_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            // Only insert Maintenances seed data if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM Maintenances WHERE Id = 1)
                BEGIN
                    SET IDENTITY_INSERT Maintenances ON;
                    INSERT INTO Maintenances (Id, CompletedDate, Cost, Description, ScheduledDate, Status, Type, VehicleId)
                    VALUES (1, '2024-01-05', 85.00, 'Regular oil change and filter replacement', '2024-01-05', 2, 0, 1);
                    SET IDENTITY_INSERT Maintenances OFF;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM Maintenances WHERE Id = 2)
                BEGIN
                    SET IDENTITY_INSERT Maintenances ON;
                    INSERT INTO Maintenances (Id, CompletedDate, Cost, Description, ScheduledDate, Status, Type, VehicleId)
                    VALUES (2, '2024-02-21', 320.00, 'Brake pad replacement and tire rotation', '2024-02-20', 2, 1, 1);
                    SET IDENTITY_INSERT Maintenances OFF;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM Maintenances WHERE Id = 3)
                BEGIN
                    SET IDENTITY_INSERT Maintenances ON;
                    INSERT INTO Maintenances (Id, CompletedDate, Cost, Description, ScheduledDate, Status, Type, VehicleId)
                    VALUES (3, '2024-03-15', 50.00, 'Annual vehicle inspection', '2024-03-15', 2, 2, 1);
                    SET IDENTITY_INSERT Maintenances OFF;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM Maintenances WHERE Id = 4)
                BEGIN
                    SET IDENTITY_INSERT Maintenances ON;
                    INSERT INTO Maintenances (Id, CompletedDate, Cost, Description, ScheduledDate, Status, Type, VehicleId)
                    VALUES (4, NULL, 150.00, 'Scheduled air conditioning service', '2024-05-01', 0, 0, 1);
                    SET IDENTITY_INSERT Maintenances OFF;
                END
            ");

            // Only insert VehicleDamages seed data if table exists and data doesn't exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'VehicleDamages')
                BEGIN
                    IF NOT EXISTS (SELECT * FROM VehicleDamages WHERE Id = 1)
                    BEGIN
                        SET IDENTITY_INSERT VehicleDamages ON;
                        INSERT INTO VehicleDamages (Id, Description, ImageUrl, RentalId, RepairCost, RepairedDate, ReportedBy, ReportedDate, Severity, Status, VehicleId)
                        VALUES (1, 'Small scratch on rear bumper, likely from parking', NULL, NULL, 150.00, '2024-02-18', 'Customer', '2024-02-15', 0, 2, 1);
                        SET IDENTITY_INSERT VehicleDamages OFF;
                    END

                    IF NOT EXISTS (SELECT * FROM VehicleDamages WHERE Id = 2)
                    BEGIN
                        SET IDENTITY_INSERT VehicleDamages ON;
                        INSERT INTO VehicleDamages (Id, Description, ImageUrl, RentalId, RepairCost, RepairedDate, ReportedBy, ReportedDate, Severity, Status, VehicleId)
                        VALUES (2, 'Dent on driver''s side door, moderate damage', NULL, NULL, 450.00, '2024-03-20', 'Admin Staff', '2024-03-12', 1, 2, 1);
                        SET IDENTITY_INSERT VehicleDamages OFF;
                    END

                    IF NOT EXISTS (SELECT * FROM VehicleDamages WHERE Id = 3)
                    BEGIN
                        SET IDENTITY_INSERT VehicleDamages ON;
                        INSERT INTO VehicleDamages (Id, Description, ImageUrl, RentalId, RepairCost, RepairedDate, ReportedBy, ReportedDate, Severity, Status, VehicleId)
                        VALUES (3, 'Windshield chip from road debris', NULL, NULL, 200.00, NULL, 'Employee', '2024-04-10', 0, 1, 1);
                        SET IDENTITY_INSERT VehicleDamages OFF;
                    END
                END
            ");

            // Update Vehicles mileage only if needed
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM Vehicles WHERE Id = 1 AND Mileage != 16000)
                BEGIN
                    UPDATE Vehicles SET Mileage = 16000 WHERE Id = 1;
                END
            ");

            // Only create indexes if not exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_UserId' AND object_id = OBJECT_ID('Rentals'))
                BEGIN
                    CREATE INDEX [IX_Rentals_UserId] ON [Rentals] ([UserId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_DriverLicenseNumber' AND object_id = OBJECT_ID('AspNetUsers'))
                BEGIN
                    CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers_DriverLicenseNumber] 
                    ON [AspNetUsers]([DriverLicenseNumber]) 
                    WHERE [DriverLicenseNumber] IS NOT NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_VehicleDamages_RentalId' AND object_id = OBJECT_ID('VehicleDamages'))
                BEGIN
                    CREATE INDEX [IX_VehicleDamages_RentalId] ON [VehicleDamages] ([RentalId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_VehicleDamages_VehicleId' AND object_id = OBJECT_ID('VehicleDamages'))
                BEGIN
                    CREATE INDEX [IX_VehicleDamages_VehicleId] ON [VehicleDamages] ([VehicleId]);
                END
            ");

            // Only add foreign key if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_AspNetUsers_UserId')
                BEGIN
                    ALTER TABLE [Rentals] ADD CONSTRAINT [FK_Rentals_AspNetUsers_UserId] 
                    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_AspNetUsers_UserId",
                table: "Rentals");

            migrationBuilder.DropTable(
                name: "VehicleDamages");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_UserId",
                table: "Rentals");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DriverLicenseNumber",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "Maintenances",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Maintenances",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Maintenances",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Maintenances",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DriverLicenseNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Tier",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Rentals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DriverLicenseNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "DateOfBirth", "DriverLicenseNumber", "Email", "FirstName", "LastName", "PhoneNumber", "RegistrationDate", "Tier" },
                values: new object[] { 1, "123 Main St, City, Country", new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL123456", "john.doe@example.com", "John", "Doe", "+1234567890", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 });

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Mileage",
                value: 15000);

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CustomerId",
                table: "Rentals",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DriverLicenseNumber",
                table: "Customers",
                column: "DriverLicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Customers_CustomerId",
                table: "Rentals",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
