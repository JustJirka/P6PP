﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NotificationService.API.Persistence.Entities.DB;

#nullable disable

namespace NotificationService.API.Migrations
{
    [DbContext(typeof(NotificationDbContext))]
    [Migration("20250408200558_MySql_1.0.7_EditTempleateSeeding")]
    partial class MySql_107_EditTempleateSeeding
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
                        .IsRequired()
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
                            Language = "en",
                            Name = "Registration",
                            Subject = "Registration confirmation",
                            Text = "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Welcome to our sports center! We are very excited that you have decided to become part of our community.</p>\r\n                            <p>To fully utilize all the features, we recommend logging in and exploring your new account.</p>\r\n                            <p>If you have any questions or need assistance, please do not hesitate to contact us.</p>\r\n\r\n                            <p>Thank you for registering and we wish you many great experiences!</p>\r\n\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>"
                        },
                        new
                        {
                            Id = 2,
                            Language = "en",
                            Name = "Verification",
                            Subject = "Account verification",
                            Text = "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Please verify your email address by clicking the following link:</p>\r\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\r\n                            <p>If you did not make this request, you can safely ignore this email.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>"
                        },
                        new
                        {
                            Id = 3,
                            Language = "en",
                            Name = "PasswordReset",
                            Subject = "Password reset",
                            Text = "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>To reset your password, click the following link:</p>\r\n                            <p><a href=\"{link}\" style=\"color: #1a73e8;\">{link}</a></p>\r\n                            <p>If you did not make this request, please ignore this email.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>"
                        },
                        new
                        {
                            Id = 4,
                            Language = "en",
                            Name = "ReservationConfirmation",
                            Subject = "Reservation confirmation",
                            Text = "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Your reservation on event {eventname}, has been created.</p>\r\n                            <p><strong>Date and time:</strong> {datetime}</p>\r\n                            <p>If you have any questions or need to modify your reservation, feel free to contact us.</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>"
                        },
                        new
                        {
                            Id = 5,
                            Language = "en",
                            Name = "ReservationCancellation",
                            Subject = "Reservation cancelation",
                            Text = "\r\n                    <html>\r\n                        <head>\r\n                            <style>\r\n                                body {\r\n                                    font-family: Arial, sans-serif;\r\n                                    font-size: 16px;\r\n                                    color: #333;\r\n                                }\r\n                                p {\r\n                                    margin: 0 0 8px 0;\r\n                                }\r\n                            </style>\r\n                        </head>\r\n                        <body>\r\n                            <p style=\"padding-bottom: 16px;\">Hello <strong>{name}</strong>,</p>\r\n\r\n                            <p>Your reservation on event{eventname}, has been canceled.</p>\r\n                            <p><strong>Date and time:</strong> {datetime}</p>\r\n\r\n                            <p>Thank you,</p>\r\n                            <p style=\"padding-top: 16px;\">Best regards,<br/>\r\n                            <em>Customer Support Team</em></p>\r\n                        </body>\r\n                    </html>"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
