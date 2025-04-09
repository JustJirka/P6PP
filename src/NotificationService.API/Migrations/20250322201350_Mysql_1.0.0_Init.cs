using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NotificationService.API.Migrations
{
    /// <inheritdoc />
    public partial class Mysql_100_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subject = table.Column<string>(type: "varchar(75)", maxLength: 75, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Text = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Language = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Templates",
                columns: new[] { "Id", "Language", "Name", "Subject", "Text" },
                values: new object[,]
                {
                    { 2, "cs", "Verification", "Ověření e-mailu", "\r\n                    <html>\r\n                        <body style=\"font-family: Arial, sans-serif; font-size: 16px; color: #333;\">\r\n                            <p>Dobrý den <strong>{name}</strong>,</p>\r\n                            <p>&nbsp;</p>\r\n                            <p>Vítejte v našem sportcentru! Máme velkou radost, že jste se rozhodli stát se součástí naší komunity.</p>\r\n                            <p>Abyste mohli naplno využívat všech možností, doporučujeme se přihlásit a prozkoumat svůj nový účet.</p>\r\n                            <p>Pokud budete mít jakékoli otázky nebo budete potřebovat pomoc, neváhejte nás kontaktovat.</p>\r\n                            <p>Děkujeme za registraci a přejeme mnoho skvělých zážitků!</p>\r\n                            <p>S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>" },
                    { 3, "cs", "PasswordReset", "Obnovení hesla", "Dobrý den {name},\n\npro obnovení hesla klikněte na následující odkaz:\n{link}\n\nPokud jste tuto žádost neodeslali, ignorujte tento e-mail.\n\nDěkujeme,\nTým podpory zákazníků" },
                    { 4, "cs", "ReservationConfirmation", "Potvrzení rezervace", "Dobrý den {name},\n\nvaše rezervace byla úspěšně vytvořena.\nDatum a čas: {datetime}\n\nPokud máte jakékoli dotazy nebo potřebujete změnit rezervaci, neváhejte nás kontaktovat.\n\nDěkujeme,\nTým podpory zákazníků" },
                    { 5, "cs", "ReservationCancellation", "Zrušení rezervace", "Dobrý den {name},\n\nvaše rezervace byla zrušena.\nDatum a čas: {datetime}\n\nPokud jste tuto žádost neodeslali, ignorujte tento e-mail.\n\nDěkujeme,\nTým podpory zákazníků" },
                    { 6, "en", "Registration", "Registration Confirmation", "Hello {name},\n\nwelcome to our sports center.We are very excited that you have decided to become part of our community.\nTo fully utilize all the features, we recommend logging in and exploring your new account.\nIf you have any questions or need assistance, please do not hesitate to contact us.\nThank you for registering and we wish you many great experiences!\n\nCustomer Support Team" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Templates");
        }
    }
}
