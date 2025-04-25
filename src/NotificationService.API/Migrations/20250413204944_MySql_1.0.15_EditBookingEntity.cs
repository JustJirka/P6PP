using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.API.Migrations
{
    /// <inheritdoc />
    public partial class MySql_1015_EditBookingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Bookings");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Please verify your email address by clicking the following link:</p>\r\n                            <p><a href=\"{token}\" style=\"color: #1a73e8;\">{token}</a></p>\r\n                            <p>If you did not make this request, you can safely ignore this email.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>To reset your password, click the following link:</p>\r\n                            <p><a href=\"{token}\" style=\"color: #1a73e8;\">{token}</a></p>\r\n                            <p>If you did not make this request, please ignore this email.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Please verify your email address by clicking the following link:</p>\r\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\r\n                            <p>If you did not make this request, you can safely ignore this email.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>To reset your password, click the following link:</p>\r\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\r\n                            <p>If you did not make this request, please ignore this email.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");
        }
    }
}
