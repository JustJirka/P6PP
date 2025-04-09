using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ServiceCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Users",
                table: "Service",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Booking",
                keyColumn: "Id",
                keyValue: 1,
                column: "BookingDate",
                value: new DateTime(2025, 4, 2, 16, 22, 40, 574, DateTimeKind.Local).AddTicks(6405));

            migrationBuilder.UpdateData(
                table: "Booking",
                keyColumn: "Id",
                keyValue: 2,
                column: "BookingDate",
                value: new DateTime(2025, 4, 2, 16, 22, 40, 574, DateTimeKind.Local).AddTicks(6410));

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ValidFrom", "ValidTo" },
                values: new object[] { new DateTime(2025, 4, 2, 16, 22, 40, 574, DateTimeKind.Local).AddTicks(5948), new DateTime(2025, 5, 2, 16, 22, 40, 574, DateTimeKind.Local).AddTicks(6009) });

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ValidFrom", "ValidTo" },
                values: new object[] { new DateTime(2025, 4, 2, 16, 22, 40, 574, DateTimeKind.Local).AddTicks(6017), new DateTime(2025, 5, 2, 16, 22, 40, 574, DateTimeKind.Local).AddTicks(6019) });

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "Id",
                keyValue: 1,
                column: "Users",
                value: "[12,5,8,3,4]");

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "Id",
                keyValue: 2,
                column: "Users",
                value: "[7,17,9,6,8,3,10,24,93,12,34,32,55,13,11]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Users",
                table: "Service");

            migrationBuilder.UpdateData(
                table: "Booking",
                keyColumn: "Id",
                keyValue: 1,
                column: "BookingDate",
                value: new DateTime(2025, 3, 27, 23, 55, 20, 191, DateTimeKind.Local).AddTicks(8617));

            migrationBuilder.UpdateData(
                table: "Booking",
                keyColumn: "Id",
                keyValue: 2,
                column: "BookingDate",
                value: new DateTime(2025, 3, 27, 23, 55, 20, 191, DateTimeKind.Local).AddTicks(8623));

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ValidFrom", "ValidTo" },
                values: new object[] { new DateTime(2025, 3, 27, 23, 55, 20, 191, DateTimeKind.Local).AddTicks(7999), new DateTime(2025, 4, 27, 23, 55, 20, 191, DateTimeKind.Local).AddTicks(8051) });

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ValidFrom", "ValidTo" },
                values: new object[] { new DateTime(2025, 3, 27, 23, 55, 20, 191, DateTimeKind.Local).AddTicks(8057), new DateTime(2025, 4, 27, 23, 55, 20, 191, DateTimeKind.Local).AddTicks(8058) });
        }
    }
}
