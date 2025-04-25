using NotificationService.API.Services;
using ReservationSystem.Shared.Results;
using NotificationService.API.Persistence.Entities.DB.Models;
using NotificationService.API.Logging; // <-- Přidáno

namespace NotificationService.API.Features;

public record GetAllTemplatesResponse(List<Template> templates);

public class GetAllTemplatesHandler
{
    private readonly TemplateAppService _templateAppService;

    public GetAllTemplatesHandler(TemplateAppService templateAppService)
    {
        _templateAppService = templateAppService;
    }

    public async Task<ApiResult<GetAllTemplatesResponse>> Handle(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var templates = await _templateAppService.GetAllTemplatesAsync();

            if (templates == null || templates.Count == 0)
            {
                var msg = "No templates found in database";
                await FileLogger.LogInfo(msg);
                return new ApiResult<GetAllTemplatesResponse>(null, false, msg);
            }

            await FileLogger.LogInfo($"Retrieved {templates.Count} templates from database");
            return new ApiResult<GetAllTemplatesResponse>(new GetAllTemplatesResponse(templates));
        }
        catch (Exception ex)
        {
            await FileLogger.LogException(ex, "Exception while retrieving templates");
            return new ApiResult<GetAllTemplatesResponse>(null, false, "An error occurred while retrieving templates");
        }
    }
}

public static class GetAllTemplatesEndpoint
{
    public static void GetAllTemplates(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notification/templates/getalltemplates",
            async (GetAllTemplatesHandler handler, CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
    }
}
