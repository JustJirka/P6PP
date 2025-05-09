using NotificationService.API.Persistence;
using System.Net.Mail;

namespace NotificationService.API.Services
{
    public interface IMailAppService
    {
        Task SendEmailAsync(EmailArgs emailArgs, IList<Attachment> attachments = null);
    }
}
