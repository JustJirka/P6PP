using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.API.Migrations
{
    /// <inheritdoc />
    public partial class EntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CheckOutDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Bookings");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInDate",
                table: "Service",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutDate",
                table: "Service",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                column: "BookingDate",
                value: new DateTime(2025, 3, 27, 19, 49, 14, 107, DateTimeKind.Local).AddTicks(364));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                column: "BookingDate",
                value: new DateTime(2025, 3, 27, 19, 49, 14, 107, DateTimeKind.Local).AddTicks(368));

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ValidFrom", "ValidTo" },
                values: new object[] { new DateTime(2025, 3, 27, 19, 49, 14, 106, DateTimeKind.Local).AddTicks(9877), new DateTime(2025, 4, 27, 19, 49, 14, 106, DateTimeKind.Local).AddTicks(9929) });

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ValidFrom", "ValidTo" },
                values: new object[] { new DateTime(2025, 3, 27, 19, 49, 14, 106, DateTimeKind.Local).AddTicks(9937), new DateTime(2025, 4, 27, 19, 49, 14, 106, DateTimeKind.Local).AddTicks(9939) });

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CheckInDate", "CheckOutDate" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInDate",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "CheckOutDate",
                table: "Service");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInDate",
                table: "Bookings",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutDate",
                table: "Bookings",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BookingDate", "CheckInDate", "CheckOutDate", "Price" },
                values: new object[] { new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1264), new DateTime(2025, 3, 25, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1268), new DateTime(2025, 3, 26, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1272), 150 });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BookingDate", "CheckInDate", "CheckOutDate", "Price" },
                values: new object[] { new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1276), new DateTime(2025, 3, 27, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1278), new DateTime(2025, 3, 28, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1280), 250 });

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ValidFrom", "ValidTo" },
                values: new object[] { new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(723), new DateTime(2025, 4, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(776) });

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ValidFrom", "ValidTo" },
                values: new object[] { new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(783), new DateTime(2025, 4, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(785) });
        }
    }
}
