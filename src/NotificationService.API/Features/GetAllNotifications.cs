using FluentValidation;
using ReservationSystem.Shared.Results;
using src.NotificationService.API.Persistence.Entities.DB.Models;
using src.NotificationService.API.Services;
using NotificationService.API.Logging; // <-- Přidáno

namespace NotificationService.API.Features;

public record GetAllNotificationsRequest(int UserId, bool unreadOnly=true, int perPage=20, int page=0);
public record GetAllNotifictionsResponse(List<NotificationLog>? NotificationLogs);

public class GetAllNotificationsValidator : AbstractValidator<GetAllNotificationsRequest>
{
    public GetAllNotificationsValidator()
    {
        RuleFor(u => u.UserId).GreaterThan(0);
        RuleFor(u => u.page).GreaterThanOrEqualTo(0);
    }
}

public class GetAllNotificationsHandler
{
    private readonly NotificationLogService _notificationLogService;

    public GetAllNotificationsHandler(NotificationLogService notificationLogService)
    {
        _notificationLogService = notificationLogService;
    }

    public async Task<ApiResult<GetAllNotifictionsResponse>> Handle(GetAllNotificationsRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var notifications = await _notificationLogService
                .GetNotificationsFor(request.UserId, request.unreadOnly, request.perPage, request.page);

            string message = "";
            if (notifications == null)
            {
                message = $"Error while getting notification logs for user {request.UserId}";
                FileLogger.LogError(message);
            }
            else if (notifications.Count == 0)
            {
                message = $"No notifications found for user {request.UserId}";
                await FileLogger.LogInfo(message);
            }
            else
            {
                message = $"Returning {notifications.Count} notifications for user {request.UserId}";
                await FileLogger.LogInfo(message);
            }

            return new ApiResult<GetAllNotifictionsResponse>(
                new GetAllNotifictionsResponse(notifications),
                notifications != null,
                message
            );
        }
        catch (Exception ex)
        {
            await FileLogger.LogException(ex, $"Exception while getting notifications for user {request.UserId}");
            return new ApiResult<GetAllNotifictionsResponse>(
                null,
                false,
                "An error occurred while retrieving notifications"
            );
        }
    }
}

public static class GetAllNotificationsEndpoint
{
    public static void GetAllNotifications(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/notification/logs/getallnotifications/{UserId:int}",
            async (int UserId, bool? unreadOnly, int? perPage, int? page,
                   GetAllNotificationsHandler handler, GetAllNotificationsValidator validator, CancellationToken cancellationToken) =>
            {
                var request = new GetAllNotificationsRequest(UserId, unreadOnly ?? true, perPage ?? 20, page ?? 0);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    await FileLogger.LogInfo($"Validation failed for GetAllNotificationsRequest: {string.Join(", ", errorMessages)}");

                    return Results.BadRequest(
                        new ApiResult<IEnumerable<string>>(
                            errorMessages,
                            false,
                            "Validation failed"
                        )
                    );
                }

                var result = await handler.Handle(request, cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            }
        );
    }
}
