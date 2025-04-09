using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomNavigationToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckOutDate",
                table: "Service",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "CheckInDate",
                table: "Service",
                newName: "End");

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Service",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                column: "BookingDate",
                value: new DateTime(2025, 3, 27, 20, 18, 53, 791, DateTimeKind.Local).AddTicks(9565));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                column: "BookingDate",
                value: new DateTime(2025, 3, 27, 20, 18, 53, 791, DateTimeKind.Local).AddTicks(9572));

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ValidFrom", "ValidTo" },
                values: new object[] { new DateTime(2025, 3, 27, 20, 18, 53, 791, DateTimeKind.Local).AddTicks(9093), new DateTime(2025, 4, 27, 20, 18, 53, 791, DateTimeKind.Local).AddTicks(9152) });

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ValidFrom", "ValidTo" },
                values: new object[] { new DateTime(2025, 3, 27, 20, 18, 53, 791, DateTimeKind.Local).AddTicks(9160), new DateTime(2025, 4, 27, 20, 18, 53, 791, DateTimeKind.Local).AddTicks(9162) });

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoomId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Service",
                keyColumn: "Id",
                keyValue: 2,
                column: "RoomId",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Service_RoomId",
                table: "Service",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Room_RoomId",
                table: "Service",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_Room_RoomId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_RoomId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Service");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Service",
                newName: "CheckOutDate");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Service",
                newName: "CheckInDate");

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
        }
    }
}
