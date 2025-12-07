using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixVehicleCategoryRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if Category column exists and rename it to CategoryId
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Vehicles]') AND name = 'Category')
                BEGIN
                    EXEC sp_rename 'Vehicles.Category', 'CategoryId', 'COLUMN';
                END
            ");

            // Drop PriceMultiplier column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Categories]') AND name = 'PriceMultiplier')
                BEGIN
                    ALTER TABLE [Categories] DROP COLUMN [PriceMultiplier];
                END
            ");

            // Insert categories only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 1)
                BEGIN
                    SET IDENTITY_INSERT Categories ON;
                    INSERT INTO Categories (Id, Name, Description, IsActive, DisplayOrder, CreatedAt)
                    VALUES (1, 'Economy', 'Fuel-efficient and budget-friendly vehicles perfect for city driving', 1, 1, '2024-01-01');
                    SET IDENTITY_INSERT Categories OFF;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 2)
                BEGIN
                    SET IDENTITY_INSERT Categories ON;
                    INSERT INTO Categories (Id, Name, Description, IsActive, DisplayOrder, CreatedAt)
                    VALUES (2, 'Compact', 'Small to medium-sized vehicles that are easy to park and maneuver', 1, 2, '2024-01-01');
                    SET IDENTITY_INSERT Categories OFF;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 3)
                BEGIN
                    SET IDENTITY_INSERT Categories ON;
                    INSERT INTO Categories (Id, Name, Description, IsActive, DisplayOrder, CreatedAt)
                    VALUES (3, 'Midsize', 'Comfortable sedans with ample space for passengers and luggage', 1, 3, '2024-01-01');
                    SET IDENTITY_INSERT Categories OFF;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 4)
                BEGIN
                    SET IDENTITY_INSERT Categories ON;
                    INSERT INTO Categories (Id, Name, Description, IsActive, DisplayOrder, CreatedAt)
                    VALUES (4, 'SUV', 'Spacious sport utility vehicles ideal for families and adventures', 1, 4, '2024-01-01');
                    SET IDENTITY_INSERT Categories OFF;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 5)
                BEGIN
                    SET IDENTITY_INSERT Categories ON;
                    INSERT INTO Categories (Id, Name, Description, IsActive, DisplayOrder, CreatedAt)
                    VALUES (5, 'Luxury', 'Premium vehicles with top-tier comfort and advanced features', 1, 5, '2024-01-01');
                    SET IDENTITY_INSERT Categories OFF;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 6)
                BEGIN
                    SET IDENTITY_INSERT Categories ON;
                    INSERT INTO Categories (Id, Name, Description, IsActive, DisplayOrder, CreatedAt)
                    VALUES (6, 'Van', 'Large capacity vehicles perfect for group travel or cargo', 1, 6, '2024-01-01');
                    SET IDENTITY_INSERT Categories OFF;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 7)
                BEGIN
                    SET IDENTITY_INSERT Categories ON;
                    INSERT INTO Categories (Id, Name, Description, IsActive, DisplayOrder, CreatedAt)
                    VALUES (7, 'Sports', 'High-performance vehicles for an exhilarating driving experience', 1, 7, '2024-01-01');
                    SET IDENTITY_INSERT Categories OFF;
                END
            ");

            // Update vehicle data
            migrationBuilder.Sql(@"
                UPDATE Vehicles SET CategoryId = 2 WHERE Id = 1;
                UPDATE Vehicles SET CategoryId = 4 WHERE Id = 2;
                UPDATE Vehicles SET CategoryId = 5 WHERE Id = 3;
                UPDATE Vehicles SET CategoryId = 1 WHERE Id = 4;
            ");

            // Create index if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Vehicles]') AND name = 'IX_Vehicles_CategoryId')
                BEGIN
                    CREATE INDEX IX_Vehicles_CategoryId ON Vehicles(CategoryId);
                END
            ");

            // Add foreign key if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_Vehicles_Categories_CategoryId]') AND parent_object_id = OBJECT_ID(N'[Vehicles]'))
                BEGIN
                    ALTER TABLE [Vehicles] ADD CONSTRAINT [FK_Vehicles_Categories_CategoryId] 
                    FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Categories_CategoryId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CategoryId",
                table: "Vehicles");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Vehicles",
                newName: "Category");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceMultiplier",
                table: "Categories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Category",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Category",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 3,
                column: "Category",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 4,
                column: "Category",
                value: 0);
        }
    }
}
