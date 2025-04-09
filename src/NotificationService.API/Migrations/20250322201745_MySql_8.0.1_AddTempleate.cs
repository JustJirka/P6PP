using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.API.Migrations
{
    /// <inheritdoc />
    public partial class MySql_801_AddTempleate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Text",
                value: "Dobrý den {name},\n\nprosíme o ověření vaší e-mailové adresy kliknutím na následující odkaz:\n{link}\n\nPokud jste tuto žádost neodeslali, ignorujte tento e-mail.\n\nDěkujeme,\nTým podpory zákazníků");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 6,
                column: "Language",
                value: "cs");

            migrationBuilder.InsertData(
                table: "Templates",
                columns: new[] { "Id", "Language", "Name", "Subject", "Text" },
                values: new object[] { 1, "cs", "Registration", "Potvrzení registrace", "\r\n                    <html>\r\n                        <body style=\"font-family: Arial, sans-serif; font-size: 16px; color: #333;\">\r\n                            <p>Dobrý den <strong>{name}</strong>,</p>\r\n                            <p>&nbsp;</p>\r\n                            <p>Vítejte v našem sportcentru! Máme velkou radost, že jste se rozhodli stát se součástí naší komunity.</p>\r\n                            <p>Abyste mohli naplno využívat všech možností, doporučujeme se přihlásit a prozkoumat svůj nový účet.</p>\r\n                            <p>Pokud budete mít jakékoli otázky nebo budete potřebovat pomoc, neváhejte nás kontaktovat.</p>\r\n                            <p>Děkujeme za registraci a přejeme mnoho skvělých zážitků!</p>\r\n                            <p>S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Text",
                value: "\r\n                    <html>\r\n                        <body style=\"font-family: Arial, sans-serif; font-size: 16px; color: #333;\">\r\n                            <p>Dobrý den <strong>{name}</strong>,</p>\r\n                            <p>&nbsp;</p>\r\n                            <p>Vítejte v našem sportcentru! Máme velkou radost, že jste se rozhodli stát se součástí naší komunity.</p>\r\n                            <p>Abyste mohli naplno využívat všech možností, doporučujeme se přihlásit a prozkoumat svůj nový účet.</p>\r\n                            <p>Pokud budete mít jakékoli otázky nebo budete potřebovat pomoc, neváhejte nás kontaktovat.</p>\r\n                            <p>Děkujeme za registraci a přejeme mnoho skvělých zážitků!</p>\r\n                            <p>S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>");

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 6,
                column: "Language",
                value: "en");
        }
    }
}
