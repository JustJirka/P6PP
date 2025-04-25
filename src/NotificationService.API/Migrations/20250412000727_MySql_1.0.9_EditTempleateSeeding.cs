using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.API.Migrations
{
    /// <inheritdoc />
    public partial class MySql_109_EditTempleateSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Welcome to our sports center! We are very excited that you have decided to become part of our community.</p>\r\n                            <p>To fully utilize all the features, we recommend logging in and exploring your new account.</p>\r\n                            <p>If you have any questions or need assistance, please do not hesitate to contact us.</p>\r\n\r\n                            <p>Thank you for registering and we wish you many great experiences!</p>\r\n\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");

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

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Your reservation on event {eventname}, has been was created.</p>\r\n                            <p><strong>Date and time:</strong> {datetime}</p>\r\n                            <p>If you have any questions or need to modify your reservation, feel free to contact us.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Your reservation on event {eventname}, has been canceled.</p>\r\n                            <p><strong>Date and time:</strong> {datetime}</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Text",
                value: "\n                    <html>\n                        <head>\n                            <style>\n                                body {\n                                    font-family: Arial, sans-serif;\n                                    font-size: 16px;\n                                    color: #333;\n                                }\n                                p {\n                                    margin: 0 0 8px 0;\n                                }\n                            </style>\n                        </head>\n                        <body>\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\n\n                            <p>Welcome to our sports center! We are very excited that you have decided to become part of our community.</p>\n                            <p>To fully utilize all the features, we recommend logging in and exploring your new account.</p>\n                            <p>If you have any questions or need assistance, please do not hesitate to contact us.</p>\n\n                            <p>Thank you for registering and we wish you many great experiences!</p>\n\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\n                            <em>Customer Support Team</em></p>\n                        </body>\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Text",
                value: "\n                    <html>\n                        <head>\n                            <style>\n                                body {\n                                    font-family: Arial, sans-serif;\n                                    font-size: 16px;\n                                    color: #333;\n                                }\n                                p {\n                                    margin: 0 0 8px 0;\n                                }\n                            </style>\n                        </head>\n                        <body>\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\n\n                            <p>Please verify your email address by clicking the following link:</p>\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\n                            <p>If you did not make this request, you can safely ignore this email.</p>\n\n                            <p>Thank you,</p>\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\n                            <em>Customer Support Team</em></p>\n                        </body>\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Text",
                value: "\n                    <html>\n                        <head>\n                            <style>\n                                body {\n                                    font-family: Arial, sans-serif;\n                                    font-size: 16px;\n                                    color: #333;\n                                }\n                                p {\n                                    margin: 0 0 8px 0;\n                                }\n                            </style>\n                        </head>\n                        <body>\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\n\n                            <p>To reset your password, click the following link:</p>\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\n                            <p>If you did not make this request, please ignore this email.</p>\n\n                            <p>Thank you,</p>\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\n                            <em>Customer Support Team</em></p>\n                        </body>\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Text",
                value: "\n                    <html>\n                        <head>\n                            <style>\n                                body {\n                                    font-family: Arial, sans-serif;\n                                    font-size: 16px;\n                                    color: #333;\n                                }\n                                p {\n                                    margin: 0 0 8px 0;\n                                }\n                            </style>\n                        </head>\n                        <body>\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\n\n                            <p>Your reservation on event {eventname}, has been was created.</p>\n                            <p><strong>Date and time:</strong> {datetime}</p>\n                            <p>If you have any questions or need to modify your reservation, feel free to contact us.</p>\n\n                            <p>Thank you,</p>\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\n                            <em>Customer Support Team</em></p>\n                        </body>\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Text",
                value: "\n                    <html>\n                        <head>\n                            <style>\n                                body {\n                                    font-family: Arial, sans-serif;\n                                    font-size: 16px;\n                                    color: #333;\n                                }\n                                p {\n                                    margin: 0 0 8px 0;\n                                }\n                            </style>\n                        </head>\n                        <body>\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\n\n                            <p>Your reservation on event{eventname}, has been canceled.</p>\n                            <p><strong>Date and time:</strong> {datetime}</p>\n\n                            <p>Thank you,</p>\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\n                            <em>Customer Support Team</em></p>\n                        </body>\n                    </html>");
        }
    }
}
