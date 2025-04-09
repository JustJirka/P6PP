using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.API.Migrations
{
    /// <inheritdoc />
    public partial class MySql_104_UpdateTemplateStyle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Dobrý den <strong>{name}</strong>,</p>\r\n\r\n                            <p>Vítejte v našem sportcentru! Máme velkou radost, že jste se rozhodli stát se součástí naší komunity.</p>\r\n                            <p>Abyste mohli naplno využívat všech možností, doporučujeme se přihlásit a prozkoumat svůj nový účet.</p>\r\n                            <p>Pokud budete mít jakékoli otázky nebo budete potřebovat pomoc, neváhejte nás kontaktovat.</p>\r\n                            <p>Děkujeme za registraci a přejeme mnoho skvělých zážitků!</p>\r\n\r\n                            <p style=\"padding-top: 16px;\">S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Dobrý den <strong>{name}</strong>,</p>\r\n\r\n                            <p>Prosíme o ověření vaší e-mailové adresy kliknutím na následující odkaz:</p>\r\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\r\n                            <p>Pokud jste tuto žádost neodeslali, můžete tento e-mail klidně ignorovat.</p>\r\n\r\n                            <p>Děkujeme,</p>\r\n                            <p style=\"padding-top: 16px;\">S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Dobrý den <strong>{name}</strong>,</p>\r\n\r\n                            <p>Pro obnovení hesla klikněte na následující odkaz:</p>\r\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\r\n                            <p>Pokud jste tuto žádost neodeslali, ignorujte tento e-mail.</p>\r\n\r\n                            <p>Děkujeme,</p>\r\n                            <p style=\"padding-top: 16px;\">S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Dobrý den <strong>{name}</strong>,</p>\r\n\r\n                            <p>Vaše rezervace byla úspěšně vytvořena.</p>\r\n                            <p><strong>Datum a čas:</strong> {datetime}</p>\r\n                            <p>Pokud máte jakékoli dotazy nebo potřebujete změnit rezervaci, neváhejte nás kontaktovat.</p>\r\n\r\n                            <p>Děkujeme,</p>\r\n                            <p style=\"padding-top: 16px;\">S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Dobrý den <strong>{name}</strong>,</p>\r\n\r\n                            <p>Vaše rezervace byla zrušena.</p>\r\n                            <p><strong>Datum a čas:</strong> {datetime}</p>\r\n                            <p>Pokud jste tuto žádost neodeslali, ignorujte tento e-mail.</p>\r\n\r\n                            <p>Děkujeme,</p>\r\n                            <p style=\"padding-top: 16px;\">S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 6,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Welcome to our sports center! We are very excited that you have decided to become part of our community.</p>\r\n                            <p>To fully utilize all the features, we recommend logging in and exploring your new account.</p>\r\n                            <p>If you have any questions or need assistance, please do not hesitate to contact us.</p>\r\n\r\n                            <p>Thank you for registering and we wish you many great experiences!</p>\r\n\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <body style=\"font-family: Arial, sans-serif; font-size: 16px; color: #333;\">\r\n                            <p>Dobrý den <strong>{name}</strong>,</p>\r\n                            <p>&nbsp;</p>\r\n                            <p>Vítejte v našem sportcentru! Máme velkou radost, že jste se rozhodli stát se součástí naší komunity.</p>\r\n                            <p>Abyste mohli naplno využívat všech možností, doporučujeme se přihlásit a prozkoumat svůj nový účet.</p>\r\n                            <p>Pokud budete mít jakékoli otázky nebo budete potřebovat pomoc, neváhejte nás kontaktovat.</p>\r\n                            <p>Děkujeme za registraci a přejeme mnoho skvělých zážitků!</p>\r\n                            <p>S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Text",
                value: "Dobrý den {name},\n\nprosíme o ověření vaší e-mailové adresy kliknutím na následující odkaz:\n{link}\n\nPokud jste tuto žádost neodeslali, ignorujte tento e-mail.\n\nDěkujeme,\nTým podpory zákazníků");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 3,
                column: "Text",
                value: "Dobrý den {name},\n\npro obnovení hesla klikněte na následující odkaz:\n{link}\n\nPokud jste tuto žádost neodeslali, ignorujte tento e-mail.\n\nDěkujeme,\nTým podpory zákazníků");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 4,
                column: "Text",
                value: "Dobrý den {name},\n\nvaše rezervace byla úspěšně vytvořena.\nDatum a čas: {datetime}\n\nPokud máte jakékoli dotazy nebo potřebujete změnit rezervaci, neváhejte nás kontaktovat.\n\nDěkujeme,\nTým podpory zákazníků");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 5,
                column: "Text",
                value: "Dobrý den {name},\n\nvaše rezervace byla zrušena.\nDatum a čas: {datetime}\n\nPokud jste tuto žádost neodeslali, ignorujte tento e-mail.\n\nDěkujeme,\nTým podpory zákazníků");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 6,
                column: "Text",
                value: "Hello {name},\n\nwelcome to our sports center.We are very excited that you have decided to become part of our community.\nTo fully utilize all the features, we recommend logging in and exploring your new account.\nIf you have any questions or need assistance, please do not hesitate to contact us.\nThank you for registering and we wish you many great experiences!\n\nCustomer Support Team");
        }
    }
}
