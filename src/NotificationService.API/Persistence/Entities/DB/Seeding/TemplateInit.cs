using NotificationService.API.Persistence.Entities.DB.Models;

namespace NotificationService.API.Persistence.Entities.DB.Seeding;

public class TemplateInit
{
    public IList<Template> GetTemplates()
    {
        List<Template> templates = new List<Template>();
        templates.Add(new Template
        {
            Id = 1,
            Name = "Registration",
            Subject = "Registration confirmation",
            Text = @"
                    <html>
                        <head>
                            <style>
                                body {
                                    font-family: Arial, sans-serif;
                                    font-size: 16px;
                                    color: #333;
                                }
                                p {
                                    margin: 0 0 8px 0;
                                }
                            </style>
                        </head>
                        <body>
                            <p style=""padding-bottom: 16px;"">Hello <strong>{name}</strong>,</p>

                            <p>Welcome to our sports center! We are very excited that you have decided to become part of our community.</p>
                            <p>To fully utilize all the features, we recommend logging in and exploring your new account.</p>
                            <p>If you have any questions or need assistance, please do not hesitate to contact us.</p>

                            <p>Thank you for registering and we wish you many great experiences!</p>

                            <p style=""padding-top: 16px;"">Best regards,<br/>
                            <em>Customer Support Team</em></p>
                        </body>
                    </html>"
        });
        templates.Add(new Template
        {
            Id = 2,
            Name = "Verification",
            Subject = "Account verification",
            Text = @"
                    <html>
                        <head>
                            <style>
                                body {
                                    font-family: Arial, sans-serif;
                                    font-size: 16px;
                                    color: #333;
                                }
                                p {
                                    margin: 0 0 8px 0;
                                }
                            </style>
                        </head>
                        <body>
                            <p style=""padding-bottom: 16px;"">Hello <strong>{name}</strong>,</p>

                            <p>Please verify your email address by clicking the following link:</p>
                            <p><a href=""{link}"" style=""color: #1a73e8;"">{link}</a></p>
                            <p>If you did not make this request, you can safely ignore this email.</p>

                            <p>Thank you,</p>
                            <p style=""padding-top: 16px;"">Best regards,<br/>
                            <em>Customer Support Team</em></p>
                        </body>
                    </html>"

        });
        templates.Add(new Template
        {
            Id = 3,
            Name = "PasswordReset",
            Subject = "Password reset",
            Text = @"
                    <html>
                        <head>
                            <style>
                                body {
                                    font-family: Arial, sans-serif;
                                    font-size: 16px;
                                    color: #333;
                                }
                                p {
                                    margin: 0 0 8px 0;
                                }
                            </style>
                        </head>
                        <body>
                            <p style=""padding-bottom: 16px;"">Hello <strong>{name}</strong>,</p>

                            <p>To reset your password, click the following link:</p>
                            <p><a href=""{link}"" style=""color: #1a73e8;"">{link}</a></p>
                            <p>If you did not make this request, please ignore this email.</p>

                            <p>Thank you,</p>
                            <p style=""padding-top: 16px;"">Best regards,<br/>
                            <em>Customer Support Team</em></p>
                        </body>
                    </html>"
        });
        templates.Add(new Template
        {
            Id = 4,
            Name = "ReservationConfirmation",
            Subject = "Reservation confirmation",
            Text = @"
                    <html>
                        <head>
                            <style>
                                body {
                                    font-family: Arial, sans-serif;
                                    font-size: 16px;
                                    color: #333;
                                }
                                p {
                                    margin: 0 0 8px 0;
                                }
                            </style>
                        </head>
                        <body>
                            <p style=""padding-bottom: 16px;"">Hello <strong>{name}</strong>,</p>

                            <p>Your reservation has been successfully created.</p>
                            <p><strong>Date and time:</strong> {datetime}</p>
                            <p>If you have any questions or need to modify your reservation, feel free to contact us.</p>

                            <p>Thank you,</p>
                            <p style=""padding-top: 16px;"">Best regards,<br/>
                            <em>Customer Support Team</em></p>
                        </body>
                    </html>"
        });
        templates.Add(new Template
        {
            Id = 5,
            Name = "ReservationCancellation",
            Subject = "Reservation cancelation",
            Text = @"
                    <html>
                        <head>
                            <style>
                                body {
                                    font-family: Arial, sans-serif;
                                    font-size: 16px;
                                    color: #333;
                                }
                                p {
                                    margin: 0 0 8px 0;
                                }
                            </style>
                        </head>
                        <body>
                            <p style=""padding-bottom: 16px;"">Hello <strong>{name}</strong>,</p>

                            <p>Your reservation has been canceled.</p>
                            <p><strong>Date and time:</strong> {datetime}</p>
                            <p>If you did not submit this request, please ignore this email.</p>

                            <p>Thank you,</p>
                            <p style=""padding-top: 16px;"">Best regards,<br/>
                            <em>Customer Support Team</em></p>
                        </body>
                    </html>"
        });

        return templates;
    }
}
