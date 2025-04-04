using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.API.Migrations
{
    /// <inheritdoc />
    public partial class MySql_106_UpdateTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Language", "Subject", "Text" },
                values: new object[] { "en", "Registration confirmation", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Welcome to our sports center! We are very excited that you have decided to become part of our community.</p>\r\n                            <p>To fully utilize all the features, we recommend logging in and exploring your new account.</p>\r\n                            <p>If you have any questions or need assistance, please do not hesitate to contact us.</p>\r\n\r\n                            <p>Thank you for registering and we wish you many great experiences!</p>\r\n\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>" });

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Language", "Subject", "Text" },
                values: new object[] { "en", "Account verification", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Please verify your email address by clicking the following link:</p>\r\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\r\n                            <p>If you did not make this request, you can safely ignore this email.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>" });

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Language", "Subject", "Text" },
                values: new object[] { "en", "Password reset", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>To reset your password, click the following link:</p>\r\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\r\n                            <p>If you did not make this request, please ignore this email.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>" });

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Language", "Subject", "Text" },
                values: new object[] { "en", "Reservation confirmation", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Your reservation has been successfully created.</p>\r\n                            <p><strong>Date and time:</strong> {datetime}</p>\r\n                            <p>If you have any questions or need to modify your reservation, feel free to contact us.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>" });

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Language", "Subject", "Text" },
                values: new object[] { "en", "Reservation cancelation", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Your reservation has been canceled.</p>\r\n                            <p><strong>Date and time:</strong> {datetime}</p>\r\n                            <p>If you did not submit this request, please ignore this email.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Language", "Subject", "Text" },
                values: new object[] { "cs", "Potvrzení registrace", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Dobrý den <strong>{name}</strong>,</p>\r\n\r\n                            <p>Vítejte v našem sportcentru! Máme velkou radost, že jste se rozhodli stát se součástí naší komunity.</p>\r\n                            <p>Abyste mohli naplno využívat všech možností, doporučujeme se přihlásit a prozkoumat svůj nový účet.</p>\r\n                            <p>Pokud budete mít jakékoli otázky nebo budete potřebovat pomoc, neváhejte nás kontaktovat.</p>\r\n                            <p>Děkujeme za registraci a přejeme mnoho skvělých zážitků!</p>\r\n\r\n                            <p style=\"padding-top: 16px;\">S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>" });

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Language", "Subject", "Text" },
                values: new object[] { "cs", "Ověření e-mailu", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Dobrý den <strong>{name}</strong>,</p>\r\n\r\n                            <p>Prosíme o ověření vaší e-mailové adresy kliknutím na následující odkaz:</p>\r\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\r\n                            <p>Pokud jste tuto žádost neodeslali, můžete tento e-mail klidně ignorovat.</p>\r\n\r\n                            <p>Děkujeme,</p>\r\n                            <p style=\"padding-top: 16px;\">S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>" });

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Language", "Subject", "Text" },
                values: new object[] { "cs", "Obnovení hesla", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Dobrý den <strong>{name}</strong>,</p>\r\n\r\n                            <p>Pro obnovení hesla klikněte na následující odkaz:</p>\r\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\r\n                            <p>Pokud jste tuto žádost neodeslali, ignorujte tento e-mail.</p>\r\n\r\n                            <p>Děkujeme,</p>\r\n                            <p style=\"padding-top: 16px;\">S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>" });

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Language", "Subject", "Text" },
                values: new object[] { "cs", "Potvrzení rezervace", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Dobrý den <strong>{name}</strong>,</p>\r\n\r\n                            <p>Vaše rezervace byla úspěšně vytvořena.</p>\r\n                            <p><strong>Datum a čas:</strong> {datetime}</p>\r\n                            <p>Pokud máte jakékoli dotazy nebo potřebujete změnit rezervaci, neváhejte nás kontaktovat.</p>\r\n\r\n                            <p>Děkujeme,</p>\r\n                            <p style=\"padding-top: 16px;\">S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>" });

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Language", "Subject", "Text" },
                values: new object[] { "cs", "Zrušení rezervace", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Dobrý den <strong>{name}</strong>,</p>\r\n\r\n                            <p>Vaše rezervace byla zrušena.</p>\r\n                            <p><strong>Datum a čas:</strong> {datetime}</p>\r\n                            <p>Pokud jste tuto žádost neodeslali, ignorujte tento e-mail.</p>\r\n\r\n                            <p>Děkujeme,</p>\r\n                            <p style=\"padding-top: 16px;\">S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>" });

            migrationBuilder.InsertData(
                table: "Templates",
                columns: new[] { "Id", "Language", "Name", "Subject", "Text" },
                values: new object[] { 6, "en", "Registration", "Registration Confirmation", "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Welcome to our sports center! We are very excited that you have decided to become part of our community.</p>\r\n                            <p>To fully utilize all the features, we recommend logging in and exploring your new account.</p>\r\n                            <p>If you have any questions or need assistance, please do not hesitate to contact us.</p>\r\n\r\n                            <p>Thank you for registering and we wish you many great experiences!</p>\r\n\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>" });
        }
    }
}
