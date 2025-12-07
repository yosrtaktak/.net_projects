using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class Add35TunisianVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DeleteData(
                table: "VehicleDamages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VehicleDamages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "VehicleDamages",
                keyColumn: "Id",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Maintenances",
                columns: new[] { "Id", "CompletedDate", "Cost", "Description", "ScheduledDate", "Status", "Type", "VehicleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 85.00m, "Regular oil change and filter replacement", new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0, 1 },
                    { 2, new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 320.00m, "Brake pad replacement and tire rotation", new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, 1 },
                    { 3, new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 50.00m, "Annual vehicle inspection", new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, 1 },
                    { 4, null, 150.00m, "Scheduled air conditioning service", new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, 1 }
                });

            migrationBuilder.InsertData(
                table: "VehicleDamages",
                columns: new[] { "Id", "Description", "ImageUrl", "RentalId", "RepairCost", "RepairedDate", "ReportedBy", "ReportedDate", "Severity", "Status", "VehicleId" },
                values: new object[,]
                {
                    { 1, "Small scratch on rear bumper, likely from parking", null, null, 150.00m, new DateTime(2024, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Customer", new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2, 1 },
                    { 2, "Dent on driver's side door, moderate damage", null, null, 450.00m, new DateTime(2024, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin Staff", new DateTime(2024, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1 },
                    { 3, "Windshield chip from road debris", null, null, 200.00m, null, "Employee", new DateTime(2024, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, 1 }
                });
        }
    }
}
