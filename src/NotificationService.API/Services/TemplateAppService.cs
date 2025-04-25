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

        public async Task<List<Template>> GetAllTemplatesAsync()
        {
            var templates = await _notificationDbContext.Templates.AsNoTracking().ToListAsync();
            return templates;
        }

        public async Task<bool> EditTemplateAsync(Template template)
        {
            // Our other services refer to templates by name so we lookup by name
            var templateToEdit = await _notificationDbContext.Templates
                .AsTracking()
                .FirstOrDefaultAsync(t => t.Name == template.Name && t.Language == template.Language);

            // Nothing to edit
            if (templateToEdit == null) return false;

            // Nothing else should be changed
            templateToEdit.Text = template.Text;
            templateToEdit.Subject = template.Subject;

            await _notificationDbContext.SaveChangesAsync();
            return true;
        }
    }
}
