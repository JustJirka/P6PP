using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;
using NotificationService.API.Services;
using NotificationService.API.Persistence;

namespace NotificationService.Tests.Services
{
    public class MailAppServiceTests
    {
        /// <summary>
        /// Verifies that the SendEmailAsync method completes without throwing an exception when given valid input.
        /// This test sets required SMTP environment variables and expects no exception to be thrown.
        /// </summary>
        [Fact]
        public async Task SendEmailAsync_SendsEmail_WhenValidInput()
        {
            Environment.SetEnvironmentVariable("SMTP_HOST", "smtp.seznam.cz");
            Environment.SetEnvironmentVariable("SMTP_PORT", "25");
            Environment.SetEnvironmentVariable("SMTP_ENABLESSL", "false");
            Environment.SetEnvironmentVariable("SMTP_USERNAME", "gym@polisenskydaniel.cz");
            Environment.SetEnvironmentVariable("SMTP_PASSWORD", "2~Q-Ya5A7@vEk2B");
            Environment.SetEnvironmentVariable("SMTP_FROM", "gym@polisenskydaniel.cz");

            var service = new MailAppService();

            var args = new EmailArgs
            {
                Address = new List<string> { "recipient@example.com" },
                Subject = "Subject",
                Body = "Body"
            };

            var exception = await Record.ExceptionAsync(() => service.SendEmailAsync(args));
            Assert.Null(exception);
        }

        /// <summary>
        /// Verifies that the SendEmailAsync method does not throw an exception when there are no recipients.
        /// </summary>
        [Fact]
        public async Task SendEmailAsync_DoesNothing_WhenNoRecipients()
        {
            Environment.SetEnvironmentVariable("SMTP_FROM", "test@example.com");

            var service = new MailAppService();

            var args = new EmailArgs
            {
                Address = new List<string>(),
                Subject = "Subject",
                Body = "Body"
            };

            var exception = await Record.ExceptionAsync(() => service.SendEmailAsync(args));
            Assert.Null(exception);
        }
    }
}
