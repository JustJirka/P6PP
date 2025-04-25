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
                    <html lang=""en"">
                      <head>
                        <meta charset=""UTF-8"" />
                        <title>Registration Confirmation</title>
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                        <style>
                          body {
                            margin: 0;
                            padding: 0;
                            background-color: #e0e0e0;
                            font-family: Arial, sans-serif;
                          }

                          table {
                            padding-top: 15px;
                            width: 100%;
                            background-color: #e0e0e0;
                          }

                          .email-container {
                            width: 100%;
                            max-width: 600px;
                            background-color: #ffffff;
                            border-radius: 6px;
                            overflow: hidden;
                            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                          }

                          .email-header {
                            background-color: #ffffff;
                            padding: 20px 40px;
                            text-align: center;
                            border-bottom: 4px solid #ea2839;
                          }

                          .email-header h1 {
                            color: #ea2839;
                            margin: 0;
                            font-size: 26px;
                            text-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                          }

                          .email-body {
                            padding: 30px 40px;
                            color: #333;
                          }

                          .email-body p {
                            font-size: 16px;
                            line-height: 1.5;
                          }

                          .email-body p strong {
                            font-weight: bold;
                          }

                          .email-footer {
                            background-color: #1c394e;
                            padding: 15px 40px;
                            text-align: center;
                            color: #ffffff;
                            font-size: 13px;
                          }

                          .email-footer a {
                            color: #ffffff;
                            text-decoration: underline;
                          }

                          @media (max-width: 600px) {
                            .email-container {
                              width: 100%;
                            }

                            .email-header {
                              padding: 15px 20px;
                            }

                            .email-header h1 {
                              font-size: 22px;
                            }

                            .email-body {
                              padding: 20px;
                            }

                            .email-body p {
                              font-size: 14px;
                            }

                            .event-details {
                              font-size: 14px;
                              padding: 10px 15px;
                            }

                            .email-footer {
                              padding: 15px 20px;
                            }

                            .email-footer a {
                              font-size: 12px;
                            }
                          }

                          @media (max-width: 400px) {
                            .email-header h1 {
                              font-size: 20px;
                            }

                            .email-body p {
                              font-size: 13px;
                            }

                            .email-footer a {
                              font-size: 11px;
                            }
                          }
                        </style>
                      </head>
                      <body>
                        <table>
                          <tr>
                            <td align=""center"">
                              <table
                                width=""100%""
                                cellspacing=""0""
                                cellpadding=""0""
                                class=""email-container""
                              >
                                <tr>
                                  <td class=""email-header"">
                                    <h1>Welcome to REZERVACE+</h1>
                                  </td>
                                </tr>

                                <tr>
                                  <td class=""email-body"">
                                    <p>Hello <strong>{name}</strong>,</p>

                                    <p>
                                      Welcome to our sports center! We are very excited that you
                                      have decided to become part of our community.
                                    </p>

                                    <p>
                                      To fully utilize all the features, we recommend logging in and
                                      exploring your new account.
                                    </p>

                                    <p>
                                      If you have any questions or need assistance, please do not
                                      hesitate to contact us.
                                    </p>

                                    <p>
                                      Thank you for registering and we wish you many great
                                      experiences!
                                    </p>

                                    <p>Best regards,<br /><em>Customer Support Team</em></p>
                                  </td>
                                </tr>

                                <tr>
                                  <td class=""email-footer"">
                                    This email address is not monitored. For support, contact
                                    <a href=""mailto:gym@polisenskydaniel.cz""
                                      >gym@polisenskydaniel.cz</a
                                    >
                                  </td>
                                </tr>
                              </table>
                            </td>
                          </tr>
                        </table>
                      </body>
                    </html>"
        });
        templates.Add(new Template
        {
            Id = 2,
            Name = "Verification",
            Subject = "Account verification",
            Text = @"
                    <html lang=""en"">
                      <head>
                        <meta charset=""UTF-8"" />
                        <title>Activation Email</title>
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                        <style>
                          body {
                            margin: 0;
                            padding: 0;
                            background-color: #e0e0e0;
                            font-family: Arial, sans-serif;
                          }

                          table {
                            padding-top: 15px;
                            width: 100%;
                            background-color: #e0e0e0;
                          }

                          .email-container {
                            width: 100%;
                            max-width: 600px;
                            background-color: #ffffff;
                            border-radius: 6px;
                            overflow: hidden;
                            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                          }

                          .email-header {
                            background-color: #ffffff;
                            padding: 20px 40px;
                            text-align: center;
                            border-bottom: 4px solid #ea2839;
                          }

                          .email-header h1 {
                            color: #ea2839;
                            margin: 0;
                            font-size: 26px;
                            text-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                          }

                          .email-body {
                            padding: 30px 40px;
                            color: #333;
                          }

                          .email-body p {
                            font-size: 16px;
                            line-height: 1.5;
                          }

                          .email-body p strong {
                            font-weight: bold;
                          }

                          .button-container {
                            text-align: center;
                            margin: 30px 0;
                          }

                          .button-container a {
                            background-color: #9b0528;
                            color: white;
                            padding: 12px 24px;
                            text-decoration: none;
                            font-weight: bold;
                            border-radius: 4px;
                            display: inline-block;
                          }

                          .email-footer {
                            background-color: #1c394e;
                            padding: 15px 40px;
                            text-align: center;
                            color: #ffffff;
                            font-size: 13px;
                          }

                          .email-footer a {
                            color: #ffffff;
                            text-decoration: underline;
                          }

                          @media (max-width: 600px) {
                            .email-container {
                              width: 100%;
                            }

                            .email-header {
                              padding: 15px 20px;
                            }

                            .email-header h1 {
                              font-size: 22px;
                            }

                            .email-body {
                              padding: 20px;
                            }

                            .email-body p {
                              font-size: 14px;
                            }

                            .button-container {
                              margin: 20px 0;
                            }

                            .button-container a {
                              width: 65%;
                              padding: 14px 24px;
                            }

                            .email-footer {
                              padding: 15px 20px;
                            }

                            .email-footer a {
                              font-size: 12px;
                            }
                          }

                          @media (max-width: 400px) {
                            .email-header h1 {
                              font-size: 20px;
                            }

                            .email-body p {
                              font-size: 13px;
                            }

                            .email-footer a {
                              font-size: 11px;
                            }
                          }
                        </style>
                      </head>
                      <body>
                        <table>
                          <tr>
                            <td align=""center"">
                              <table
                                width=""100%""
                                cellspacing=""0""
                                cellpadding=""0""
                                class=""email-container""
                              >
                                <tr>
                                  <td class=""email-header"">
                                    <h1>Welcome to REZERVACE+</h1>
                                  </td>
                                </tr>

                                <tr>
                                  <td class=""email-body"">
                                    <p>Hello <strong>{name}</strong>,</p>

                                    <p>
                                      Thank you for registering! To confirm your email address,
                                      please click the button below:
                                    </p>

                                    <div class=""button-container"">
                                      <a href=""http://localhost:4201/email_verification?userId={userId}&token={token}"">Activate Account</a>
                                    </div>

                                    <p>
                                      If you didn't request this, you can safely ignore this email.
                                    </p>

                                    <p>Thanks,<br /><em>Customer Support Team</em></p>
                                  </td>
                                </tr>

                                <tr>
                                  <td class=""email-footer"">
                                    This email address is not monitored. For support, contact
                                    <a href=""mailto:gym@polisenskydaniel.cz""
                                      >gym@polisenskydaniel.cz</a
                                    >
                                  </td>
                                </tr>
                              </table>
                            </td>
                          </tr>
                        </table>
                      </body>
                    </html>"

        });
        templates.Add(new Template
        {
            Id = 3,
            Name = "PasswordReset",
            Subject = "Password reset",
            Text = @"
                    <html lang=""en"">
                      <head>
                        <meta charset=""UTF-8"" />
                        <title>Password Reset</title>
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                        <style>
                          body {
                            margin: 0;
                            padding: 0;
                            background-color: #e0e0e0;
                            font-family: Arial, sans-serif;
                          }

                          table {
                            padding-top: 15px;
                            width: 100%;
                            background-color: #e0e0e0;
                          }

                          .email-container {
                            width: 100%;
                            max-width: 600px;
                            background-color: #ffffff;
                            border-radius: 6px;
                            overflow: hidden;
                            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                          }

                          .email-header {
                            background-color: #ffffff;
                            padding: 20px 40px;
                            text-align: center;
                            border-bottom: 4px solid #ea2839;
                          }

                          .email-header h1 {
                            color: #ea2839;
                            margin: 0;
                            font-size: 26px;
                            text-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                          }

                          .email-body {
                            padding: 30px 40px;
                            color: #333;
                          }

                          .email-body p {
                            font-size: 16px;
                            line-height: 1.5;
                          }

                          .email-body p strong {
                            font-weight: bold;
                          }

                          .button-container {
                            text-align: center;
                            margin: 30px 0;
                          }

                          .button-container a {
                            background-color: #9b0528;
                            color: white !important;
                            padding: 12px 24px;
                            text-decoration: none;
                            font-weight: bold;
                            border-radius: 4px;
                            display: inline-block;
                          }

                          .email-footer {
                            background-color: #1c394e;
                            padding: 15px 40px;
                            text-align: center;
                            color: #ffffff;
                            font-size: 13px;
                          }

                          .email-footer a {
                            color: #ffffff;
                            text-decoration: underline;
                          }

                          @media (max-width: 600px) {
                            .email-container {
                              width: 100%;
                            }

                            .email-header {
                              padding: 15px 20px;
                            }

                            .email-header h1 {
                              font-size: 22px;
                            }

                            .email-body {
                              padding: 20px;
                            }

                            .email-body p {
                              font-size: 14px;
                            }

                            .button-container {
                              margin: 20px 0;
                            }

                            .button-container a {
                              width: 65%;
                              padding: 14px 24px;
                            }

                            .email-footer {
                              padding: 15px 20px;
                            }

                            .email-footer a {
                              font-size: 12px;
                            }
                          }

                          @media (max-width: 400px) {
                            .email-header h1 {
                              font-size: 20px;
                            }

                            .email-body p {
                              font-size: 13px;
                            }

                            .email-footer a {
                              font-size: 11px;
                            }
                          }
                        </style>
                      </head>
                      <body>
                        <table>
                          <tr>
                            <td align=""center"">
                              <table
                                width=""100%""
                                cellspacing=""0""
                                cellpadding=""0""
                                class=""email-container""
                              >
                                <tr>
                                  <td class=""email-header"">
                                    <h1>Password Reset Request</h1>
                                  </td>
                                </tr>

                                <tr>
                                  <td class=""email-body"">
                                    <p>Hello <strong>{name}</strong>,</p>

                                    <p>
                                      We received a request to reset your password. Click the button
                                      below to proceed:
                                    </p>

                                    <div class=""button-container"">
                                      <a href=""http://localhost:4201/password_reset?userId={userId}&token={token}"" rel=""noopener noreferrer""
                                        >Reset Password</a
                                      >
                                    </div>

                                    <p>
                                      If you didn’t request this, you can safely ignore this email.
                                      Your password will remain unchanged.
                                    </p>

                                    <p>Thank you,<br /><em>Customer Support Team</em></p>
                                  </td>
                                </tr>

                                <tr>
                                  <td class=""email-footer"">
                                    This email address is not monitored. For support, contact
                                    <a href=""mailto:gym@polisenskydaniel.cz""
                                      >gym@polisenskydaniel.cz</a
                                    >
                                  </td>
                                </tr>
                              </table>
                            </td>
                          </tr>
                        </table>
                      </body>
                    </html>"
        });
        templates.Add(new Template
        {
            Id = 4,
            Name = "BookingConfirmation",
            Subject = "Reservation confirmation",
            Text = @"
                    <html lang=""en"">
                        <head>
                        <meta charset=""UTF-8"" />
                        <title>Booking Confirmation</title>
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                        <style>
                            body {
                            margin: 0;
                            padding: 0;
                            background-color: #e0e0e0;
                            font-family: Arial, sans-serif;
                            }

                            table {
                            padding-top: 15px;
                            width: 100%;
                            background-color: #e0e0e0;
                            }

                            .email-container {
                            width: 100%;
                            max-width: 600px;
                            background-color: #ffffff;
                            border-radius: 6px;
                            overflow: hidden;
                            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                            }

                            .email-header {
                            background-color: #ffffff;
                            padding: 20px 40px;
                            text-align: center;
                            border-bottom: 4px solid #ea2839;
                            }

                            .email-header h1 {
                            color: #ea2839;
                            margin: 0;
                            font-size: 26px;
                            text-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                            }

                            .email-body {
                            padding: 30px 40px;
                            color: #333;
                            }

                            .email-body p {
                            font-size: 16px;
                            line-height: 1.5;
                            }

                            .email-body p strong {
                            font-weight: bold;
                            }

                            .event-details {
                            margin: 20px 0;
                            padding: 12px 20px;
                            background-color: #f5f5f5;
                            border-radius: 4px;
                            font-size: 16px;
                            line-height: 1.5;
                            }

                            .email-footer {
                            background-color: #1c394e;
                            padding: 15px 40px;
                            text-align: center;
                            color: #ffffff;
                            font-size: 13px;
                            }

                            .email-footer a {
                            color: #ffffff;
                            text-decoration: underline;
                            }

                            @media (max-width: 600px) {
                            .email-container {
                                width: 100%;
                            }

                            .email-header {
                                padding: 15px 20px;
                            }

                            .email-header h1 {
                                font-size: 22px;
                            }

                            .email-body {
                                padding: 20px;
                            }

                            .email-body p {
                                font-size: 14px;
                            }

                            .event-details {
                                font-size: 14px;
                                padding: 10px 15px;
                            }

                            .email-footer {
                                padding: 15px 20px;
                            }

                            .email-footer a {
                                font-size: 12px;
                            }
                            }

                            @media (max-width: 400px) {
                            .email-header h1 {
                                font-size: 20px;
                            }

                            .email-body p {
                                font-size: 13px;
                            }

                            .email-footer a {
                                font-size: 11px;
                            }
                            }
                        </style>
                        </head>
                        <body>
                        <table>
                            <tr>
                            <td align=""center"">
                                <table
                                width=""100%""
                                cellspacing=""0""
                                cellpadding=""0""
                                class=""email-container""
                                >
                                <tr>
                                    <td class=""email-header"">
                                    <h1>Booking Confirmation</h1>
                                    </td>
                                </tr>

                                <tr>
                                    <td class=""email-body"">
                                    <p>Hello <strong>{name}</strong>,</p>

                                    <p>
                                        Your reservation for the event
                                        <strong>{eventname}</strong> has been successfully created.
                                    </p>

                                    <div class=""event-details"">
                                        <strong>Date and Time:</strong><br />
                                        {datetime}
                                    </div>

                                    <p>
                                        If you have any questions or need to modify your reservation,
                                        feel free to contact us.
                                    </p>

                                    <p>Thank you,<br /><em>Customer Support Team</em></p>
                                    </td>
                                </tr>

                                <tr>
                                    <td class=""email-footer"">
                                    This email address is not monitored. For support, contact
                                    <a href=""mailto:gym@polisenskydaniel.cz""
                                        >gym@polisenskydaniel.cz</a
                                    >
                                    </td>
                                </tr>
                                </table>
                            </td>
                            </tr>
                        </table>
                        </body>
                    </html>"
        });
        templates.Add(new Template
        {
            Id = 5,
            Name = "BookingCancellation",
            Subject = "Reservation cancelation",
            Text = @"
                    <html lang=""en"">
                        <head>
                        <meta charset=""UTF-8"" />
                        <title>Booking Cancelled</title>
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                        <style>
                            body {
                            margin: 0;
                            padding: 0;
                            background-color: #e0e0e0;
                            font-family: Arial, sans-serif;
                            }

                            table {
                            padding-top: 15px;
                            width: 100%;
                            background-color: #e0e0e0;
                            }

                            .email-container {
                            width: 100%;
                            max-width: 600px;
                            background-color: #ffffff;
                            border-radius: 6px;
                            overflow: hidden;
                            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                            }

                            .email-header {
                            background-color: #ffffff;
                            padding: 20px 40px;
                            text-align: center;
                            border-bottom: 4px solid #ea2839;
                            }

                            .email-header h1 {
                            color: #ea2839;
                            margin: 0;
                            font-size: 26px;
                            text-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                            }

                            .email-body {
                            padding: 30px 40px;
                            color: #333;
                            }

                            .email-body p {
                            font-size: 16px;
                            line-height: 1.5;
                            }

                            .email-body p strong {
                            font-weight: bold;
                            }

                            .event-details {
                            margin: 20px 0;
                            padding: 12px 20px;
                            background-color: #f5f5f5;
                            border-radius: 4px;
                            font-size: 16px;
                            line-height: 1.5;
                            }

                            .email-footer {
                            background-color: #1c394e;
                            padding: 15px 40px;
                            text-align: center;
                            color: #ffffff;
                            font-size: 13px;
                            }

                            .email-footer a {
                            color: #ffffff;
                            text-decoration: underline;
                            }

                            @media (max-width: 600px) {
                            .email-container {
                                width: 100%;
                            }

                            .email-header {
                                padding: 15px 20px;
                            }

                            .email-header h1 {
                                font-size: 22px;
                            }

                            .email-body {
                                padding: 20px;
                            }

                            .email-body p {
                                font-size: 14px;
                            }

                            .event-details {
                                font-size: 14px;
                                padding: 10px 15px;
                            }

                            .email-footer {
                                padding: 15px 20px;
                            }

                            .email-footer a {
                                font-size: 12px;
                            }
                            }

                            @media (max-width: 400px) {
                            .email-header h1 {
                                font-size: 20px;
                            }

                            .email-body p {
                                font-size: 13px;
                            }

                            .email-footer a {
                                font-size: 11px;
                            }
                            }
                        </style>
                        </head>
                        <body>
                        <table>
                            <tr>
                            <td align=""center"">
                                <table
                                width=""100%""
                                cellspacing=""0""
                                cellpadding=""0""
                                class=""email-container""
                                >
                                <tr>
                                    <td class=""email-header"">
                                    <h1>Booking Cancelled</h1>
                                    </td>
                                </tr>

                                <tr>
                                    <td class=""email-body"">
                                    <p>Hello <strong>{name}</strong>,</p>

                                    <p>
                                        Your reservation for the event
                                        <strong>{eventname}</strong> has been cancelled.
                                    </p>

                                    <div class=""event-details"">
                                        <strong>Date and Time:</strong><br />
                                        {datetime}
                                    </div>

                                    <p>
                                        If this was a mistake or you have questions, please reach out
                                        to our support team.
                                    </p>

                                    <p>Thank you,<br /><em>Customer Support Team</em></p>
                                    </td>
                                </tr>

                                <tr>
                                    <td class=""email-footer"">
                                    This email address is not monitored. For support, contact
                                    <a href=""mailto:gym@polisenskydaniel.cz""
                                        >gym@polisenskydaniel.cz</a
                                    >
                                    </td>
                                </tr>
                                </table>
                            </td>
                            </tr>
                        </table>
                        </body>
                    </html>"
        });

        templates.Add(new Template
        {
            Id = 6,
            Name = "BookingReminder",
            Subject = "Reservation reminder",
            Text = @"
                    <html lang=""en"">
                        <head>
                        <meta charset=""UTF-8"" />
                        <title>Upcoming Event Reminder</title>
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                        <style>
                            body {
                            margin: 0;
                            padding: 0;
                            background-color: #e0e0e0;
                            font-family: Arial, sans-serif;
                            }

                            table {
                            padding-top: 15px;
                            width: 100%;
                            background-color: #e0e0e0;
                            }

                            .email-container {
                            width: 100%;
                            max-width: 600px;
                            background-color: #ffffff;
                            border-radius: 6px;
                            overflow: hidden;
                            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                            }

                            .email-header {
                            background-color: #ffffff;
                            padding: 20px 40px;
                            text-align: center;
                            border-bottom: 4px solid #ea2839;
                            }

                            .email-header h1 {
                            color: #ea2839;
                            margin: 0;
                            font-size: 26px;
                            text-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                            }

                            .email-body {
                            padding: 30px 40px;
                            color: #333;
                            }

                            .email-body p {
                            font-size: 16px;
                            line-height: 1.5;
                            }

                            .email-body p strong {
                            font-weight: bold;
                            }

                            .event-details {
                            margin: 20px 0;
                            padding: 12px 20px;
                            background-color: #f5f5f5;
                            border-radius: 4px;
                            font-size: 16px;
                            line-height: 1.5;
                            }

                            .email-footer {
                            background-color: #1c394e;
                            padding: 15px 40px;
                            text-align: center;
                            color: #ffffff;
                            font-size: 13px;
                            }

                            .email-footer a {
                            color: #ffffff;
                            text-decoration: underline;
                            }

                            @media (max-width: 600px) {
                            .email-container {
                                width: 100%;
                            }

                            .email-header {
                                padding: 15px 20px;
                            }

                            .email-header h1 {
                                font-size: 22px;
                            }

                            .email-body {
                                padding: 20px;
                            }

                            .email-body p {
                                font-size: 14px;
                            }

                            .event-details {
                                font-size: 14px;
                                padding: 10px 15px;
                            }

                            .email-footer {
                                padding: 15px 20px;
                            }

                            .email-footer a {
                                font-size: 12px;
                            }
                            }

                            @media (max-width: 400px) {
                            .email-header h1 {
                                font-size: 20px;
                            }

                            .email-body p {
                                font-size: 13px;
                            }

                            .email-footer a {
                                font-size: 11px;
                            }
                            }
                        </style>
                        </head>
                        <body>
                        <table>
                            <tr>
                            <td align=""center"">
                                <table
                                width=""100%""
                                cellspacing=""0""
                                cellpadding=""0""
                                class=""email-container""
                                >
                                <tr>
                                    <td class=""email-header"">
                                    <h1>Event Reminder: {eventname}</h1>
                                    </td>
                                </tr>

                                <tr>
                                    <td class=""email-body"">
                                    <p>Hello <strong>{name}</strong>,</p>

                                    <p>
                                        This is a reminder that your upcoming event
                                        <strong>{eventname}</strong> is just around the corner!
                                    </p>

                                    <div class=""event-details"">
                                        <strong>Date and Time:</strong><br />
                                        {datetime}
                                    </div>

                                    <p>
                                        We are looking forward to seeing you there. If you have any
                                        questions or need further assistance, feel free to contact us.
                                    </p>

                                    <p>Best regards,<br /><em>Customer Support Team</em></p>
                                    </td>
                                </tr>

                                <tr>
                                    <td class=""email-footer"">
                                    This email address is not monitored. For support, contact
                                    <a href=""mailto:gym@polisenskydaniel.cz""
                                        >gym@polisenskydaniel.cz</a
                                    >
                                    </td>
                                </tr>
                                </table>
                            </td>
                            </tr>
                        </table>
                        </body>
                    </html>"
        });

        return templates;
    }
}
