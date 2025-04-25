using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.API.Migrations
{
    /// <inheritdoc />
    public partial class MySql_107_EditTempleateSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Your reservation on event {eventname}, has been created.</p>\r\n                            <p><strong>Date and time:</strong> {datetime}</p>\r\n                            <p>If you have any questions or need to modify your reservation, feel free to contact us.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Your reservation on event{eventname}, has been canceled.</p>\r\n                            <p><strong>Date and time:</strong> {datetime}</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Your reservation has been successfully created.</p>\r\n                            <p><strong>Date and time:</strong> {datetime}</p>\r\n                            <p>If you have any questions or need to modify your reservation, feel free to contact us.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Your reservation has been canceled.</p>\r\n                            <p><strong>Date and time:</strong> {datetime}</p>\r\n                            <p>If you did not submit this request, please ignore this email.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");
        }
    }
}
