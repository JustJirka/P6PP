using NotificationService.API.Persistence.Entities.DB;
using NotificationService.API.Persistence.Entities.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.API.Services
{
    public class TemplateAppService
    {
        private readonly NotificationDbContext _notificationDbContext;

        public TemplateAppService(NotificationDbContext notificationDbContext)
        {
            _notificationDbContext = notificationDbContext;
        }

        public async Task<Template> GetTemplateAsync(string name, string? language = "en")
        {
            if (string.IsNullOrEmpty(language))
            {
                language = "en";
            }
            var template = await _notificationDbContext.Templates
                .FirstAsync(x => x.Name == name && x.Language == language);

            return template;
        }

        public async Task EditTemplateAsync(Template template)
        {
            _notificationDbContext.Templates.Update(template);
            await _notificationDbContext.SaveChangesAsync();
        }
    }
}
