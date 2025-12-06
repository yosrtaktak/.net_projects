using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class LinkVehicleToCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Vehicles",
                newName: "CategoryId");

            // Insert categories only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 1)
                BEGIN
                    SET IDENTITY_INSERT Categories ON;
                    INSERT INTO Categories (Id, Name, Description, IsActive, DisplayOrder, CreatedAt)
                    VALUES (1, 'Economy', 'Fuel-efficient and budget-friendly vehicles perfect for city driving', 1, 1, '2024-01-01');
                    SET IDENTITY_INSERT Categories OFF;
                END
                ELSE
                BEGIN
                    UPDATE Categories SET 
                        Name = 'Economy',
                        Description = 'Fuel-efficient and budget-friendly vehicles perfect for city driving',
                        DisplayOrder = 1
                    WHERE Id = 1;
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
                ELSE
                BEGIN
                    UPDATE Categories SET 
                        Name = 'Compact',
                        Description = 'Small to medium-sized vehicles that are easy to park and maneuver',
                        DisplayOrder = 2
                    WHERE Id = 2;
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
                ELSE
                BEGIN
                    UPDATE Categories SET 
                        Name = 'Midsize',
                        Description = 'Comfortable sedans with ample space for passengers and luggage',
                        DisplayOrder = 3
                    WHERE Id = 3;
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
                ELSE
                BEGIN
                    UPDATE Categories SET 
                        Name = 'SUV',
                        Description = 'Spacious sport utility vehicles ideal for families and adventures',
                        DisplayOrder = 4
                    WHERE Id = 4;
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
                ELSE
                BEGIN
                    UPDATE Categories SET 
                        Name = 'Luxury',
                        Description = 'Premium vehicles with top-tier comfort and advanced features',
                        DisplayOrder = 5
                    WHERE Id = 5;
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
                ELSE
                BEGIN
                    UPDATE Categories SET 
                        Name = 'Van',
                        Description = 'Large capacity vehicles perfect for group travel or cargo',
                        DisplayOrder = 6
                    WHERE Id = 6;
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
                ELSE
                BEGIN
                    UPDATE Categories SET 
                        Name = 'Sports',
                        Description = 'High-performance vehicles for an exhilarating driving experience',
                        DisplayOrder = 7
                    WHERE Id = 7;
                END
            ");

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoryId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CategoryId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CategoryId",
                table: "Vehicles",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Categories_CategoryId",
                table: "Vehicles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
