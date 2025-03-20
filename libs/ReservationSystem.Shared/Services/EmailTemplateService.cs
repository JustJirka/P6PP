namespace ReservationSystem.Shared.Services;

using System.IO;
using System.Threading.Tasks;
using RazorEngineCore;

public class EmailTemplateService
{
    private readonly string _templatePath = "libs/ReservationSystem.Shared/Templates/Emails/";

    public async Task<string> RenderTemplateAsync<T>(string templateName, T model)
    {
        var templateFile = Path.Combine(_templatePath, $"{templateName}.cshtml");

        if (!File.Exists(templateFile))
            throw new FileNotFoundException($"Template {templateName} not found at {templateFile}");

        var templateText = await File.ReadAllTextAsync(templateFile);
        var razorEngine = new RazorEngine();
        var compiledTemplate = await razorEngine.CompileAsync(templateText);
        
        return await compiledTemplate.RunAsync(model);
    }
}