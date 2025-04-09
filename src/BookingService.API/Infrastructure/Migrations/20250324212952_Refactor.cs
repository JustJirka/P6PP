using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingService.API.Migrations
{
    /// <inheritdoc />
    public partial class Refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingStatus");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "RoomStatus");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Bookings",
                newName: "Status");

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Percentage = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    ServiceName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsCancelled = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BookingDate", "CheckInDate", "CheckOutDate", "Status" },
                values: new object[] { new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1264), new DateTime(2025, 3, 25, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1268), new DateTime(2025, 3, 26, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1272), 0 });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BookingDate", "CheckInDate", "CheckOutDate", "Status" },
                values: new object[] { new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1276), new DateTime(2025, 3, 27, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1278), new DateTime(2025, 3, 28, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(1280), 1 });

            migrationBuilder.InsertData(
                table: "Discount",
                columns: new[] { "Id", "IsValid", "Percentage", "ValidFrom", "ValidTo" },
                values: new object[,]
                {
                    { 1, true, 10, new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(723), new DateTime(2025, 4, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(776) },
                    { 2, true, 20, new DateTime(2025, 3, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(783), new DateTime(2025, 4, 24, 22, 29, 51, 518, DateTimeKind.Local).AddTicks(785) }
                });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Id", "Capacity", "Name", "Status" },
                values: new object[,]
                {
                    { 1, 20, "Room A", 0 },
                    { 2, 15, "Room B", 1 },
                    { 3, 10, "Room C", 2 },
                    { 4, 25, "Room D", 3 }
                });

            migrationBuilder.InsertData(
                table: "Service",
                columns: new[] { "Id", "IsCancelled", "Price", "ServiceName", "TrainerId" },
                values: new object[,]
                {
                    { 1, false, 100, "Service A", 1 },
                    { 2, false, 200, "Service B", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Bookings",
                newName: "StatusId");

            migrationBuilder.CreateTable(
                name: "BookingStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BookingStatusName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStatus", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscountPercentage = table.Column<int>(type: "int", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RoomStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomStatus", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsCancelled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    ServiceName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TrainerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    RoomCapacity = table.Column<int>(type: "int", nullable: false),
                    RoomName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_RoomStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "RoomStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "BookingStatus",
                columns: new[] { "Id", "BookingStatusName" },
                values: new object[,]
                {
                    { 1, "Confirmed" },
                    { 2, "Pending" },
                    { 3, "Cancelled" }
                });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BookingDate", "CheckInDate", "CheckOutDate", "StatusId" },
                values: new object[] { new DateTime(2025, 3, 23, 14, 2, 47, 833, DateTimeKind.Local).AddTicks(7493), new DateTime(2025, 3, 24, 14, 2, 47, 833, DateTimeKind.Local).AddTicks(7498), new DateTime(2025, 3, 25, 14, 2, 47, 833, DateTimeKind.Local).AddTicks(7503), 1 });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BookingDate", "CheckInDate", "CheckOutDate", "StatusId" },
                values: new object[] { new DateTime(2025, 3, 23, 14, 2, 47, 833, DateTimeKind.Local).AddTicks(7514), new DateTime(2025, 3, 26, 14, 2, 47, 833, DateTimeKind.Local).AddTicks(7517), new DateTime(2025, 3, 27, 14, 2, 47, 833, DateTimeKind.Local).AddTicks(7519), 2 });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "Id", "DiscountPercentage", "IsValid", "ValidFrom", "ValidTo" },
                values: new object[,]
                {
                    { 1, 10, true, new DateTime(2025, 3, 23, 14, 2, 47, 833, DateTimeKind.Local).AddTicks(7129), new DateTime(2025, 4, 23, 14, 2, 47, 833, DateTimeKind.Local).AddTicks(7206) },
                    { 2, 20, true, new DateTime(2025, 3, 23, 14, 2, 47, 833, DateTimeKind.Local).AddTicks(7219), new DateTime(2025, 4, 23, 14, 2, 47, 833, DateTimeKind.Local).AddTicks(7221) }
                });

            migrationBuilder.InsertData(
                table: "RoomStatus",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { 1, "Available" },
                    { 2, "Occupied" },
                    { 3, "Reserved" },
                    { 4, "Maintenance" }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "IsCancelled", "Price", "ServiceName", "TrainerId" },
                values: new object[,]
                {
                    { 1, false, 100, "Service A", 1 },
                    { 2, false, 200, "Service B", 2 }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "RoomCapacity", "RoomName", "StatusId" },
                values: new object[,]
                {
                    { 1, 20, "Room A", 1 },
                    { 2, 15, "Room B", 2 },
                    { 3, 10, "Room C", 3 },
                    { 4, 25, "Room D", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_StatusId",
                table: "Rooms",
                column: "StatusId");
        }
    }
}
