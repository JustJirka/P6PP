using NotificationService.API.Persistence.Entities.DB.Models;

namespace NotificationService.API.Abstraction
{
    public interface ITemplateAppService
    {
        Task<Template> GetTemplateAsync(string name, string? language = "en");
        Task<List<Template>> GetAllTemplatesAsync();
        Task<bool> EditTemplateAsync(Template template);
    }
}
