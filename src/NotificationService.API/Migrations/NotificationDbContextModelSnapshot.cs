﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NotificationService.API.Persistence.Entities.DB;

#nullable disable

namespace NotificationService.API.Migrations
{
    [DbContext(typeof(NotificationDbContext))]
    partial class NotificationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("NotificationService.API.Persistence.Entities.DB.Models.Template", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Subject")
                        .HasMaxLength(75)
                        .HasColumnType("varchar(75)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(1500)
                        .HasColumnType("varchar(1500)");

                    b.HasKey("Id");

                    b.ToTable("Templates");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Language = "cs",
                            Name = "Registration",
                            Subject = "Potvrzení registrace",
                            Text = "\r\n                    <html>\r\n                        <body style=\"font-family: Arial, sans-serif; font-size: 16px; color: #333;\">\r\n                            <p>Dobrý den <strong>{name}</strong>,</p>\r\n                            <p>&nbsp;</p>\r\n                            <p>Vítejte v našem sportcentru! Máme velkou radost, že jste se rozhodli stát se součástí naší komunity.</p>\r\n                            <p>Abyste mohli naplno využívat všech možností, doporučujeme se přihlásit a prozkoumat svůj nový účet.</p>\r\n                            <p>Pokud budete mít jakékoli otázky nebo budete potřebovat pomoc, neváhejte nás kontaktovat.</p>\r\n                            <p>Děkujeme za registraci a přejeme mnoho skvělých zážitků!</p>\r\n                            <p>S pozdravem,<br/>\r\n                            <em>Tým podpory zákazníků</em></p>\r\n                        </body>\r\n                    </html>"
                        },
                        new
                        {
                            Id = 2,
                            Language = "cs",
                            Name = "Verification",
                            Subject = "Ověření e-mailu",
                            Text = "Dobrý den {name},\n\nprosíme o ověření vaší e-mailové adresy kliknutím na následující odkaz:\n{link}\n\nPokud jste tuto žádost neodeslali, ignorujte tento e-mail.\n\nDěkujeme,\nTým podpory zákazníků"
                        },
                        new
                        {
                            Id = 3,
                            Language = "cs",
                            Name = "PasswordReset",
                            Subject = "Obnovení hesla",
                            Text = "Dobrý den {name},\n\npro obnovení hesla klikněte na následující odkaz:\n{link}\n\nPokud jste tuto žádost neodeslali, ignorujte tento e-mail.\n\nDěkujeme,\nTým podpory zákazníků"
                        },
                        new
                        {
                            Id = 4,
                            Language = "cs",
                            Name = "ReservationConfirmation",
                            Subject = "Potvrzení rezervace",
                            Text = "Dobrý den {name},\n\nvaše rezervace byla úspěšně vytvořena.\nDatum a čas: {datetime}\n\nPokud máte jakékoli dotazy nebo potřebujete změnit rezervaci, neváhejte nás kontaktovat.\n\nDěkujeme,\nTým podpory zákazníků"
                        },
                        new
                        {
                            Id = 5,
                            Language = "cs",
                            Name = "ReservationCancellation",
                            Subject = "Zrušení rezervace",
                            Text = "Dobrý den {name},\n\nvaše rezervace byla zrušena.\nDatum a čas: {datetime}\n\nPokud jste tuto žádost neodeslali, ignorujte tento e-mail.\n\nDěkujeme,\nTým podpory zákazníků"
                        },
                        new
                        {
                            Id = 6,
                            Language = "en",
                            Name = "Registration",
                            Subject = "Registration Confirmation",
                            Text = "Hello {name},\n\nwelcome to our sports center.We are very excited that you have decided to become part of our community.\nTo fully utilize all the features, we recommend logging in and exploring your new account.\nIf you have any questions or need assistance, please do not hesitate to contact us.\nThank you for registering and we wish you many great experiences!\n\nCustomer Support Team"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
