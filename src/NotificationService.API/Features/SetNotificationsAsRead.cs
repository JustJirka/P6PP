using ReservationSystem.Shared.Results;
using NotificationService.API.Logging;
using src.NotificationService.API.Services;

namespace NotificationService.API.Features;

public record SetAllNotificationsAsReadRequest(int UserId);
public record SetSomeNotificationsAsReadRequest(List<int> notificationIds);

public record SetNotificationsResponse(int NotificationsReadStatusChanged);

public class SetNotificationsHandler
{
    private readonly NotificationLogService _notifciationLogService;

    public SetNotificationsHandler(NotificationLogService notifciationLogService)
    {
        _notifciationLogService = notifciationLogService;
    }

    public async Task<ApiResult<SetNotificationsResponse>> HandleAllNotificationsAsRead(
        SetAllNotificationsAsReadRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var changed = await _notifciationLogService.SetAllNotificationsAsRead(request.UserId);

            await FileLogger.LogInfo($"Set all notifications for UserId {request.UserId} as read");
            return new ApiResult<SetNotificationsResponse>(new SetNotificationsResponse(changed), true);
        }
        catch (Exception ex)
        {
            await FileLogger.LogException(ex, "Exception while settings notifications as read");
            return new ApiResult<SetNotificationsResponse>(null, false, "An unknown error occurred while settings notifications as read");
        }
    }

    public async Task<ApiResult<SetNotificationsResponse>> HandleSomeNotificationsAsRead(
        SetSomeNotificationsAsReadRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var changed = await _notifciationLogService.SetSomeNotificationsAsRead(request.notificationIds);

            await FileLogger.LogInfo($"Set {request.notificationIds} notifications as read");
            return new ApiResult<SetNotificationsResponse>(new SetNotificationsResponse(changed), true);
        }
        catch (Exception ex)
        {
            await FileLogger.LogException(ex, "Exception while settings notifications as read");
            return new ApiResult<SetNotificationsResponse>(null, false, "An unknown error occurred while settings notifications as read");
        }
    }
}

public static class SetNotificationsAsReadEndpoint
{
    public static void SetAllNotificationsAsRead(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/notification/logs/setallnotificationsasread/{UserId}",
                   async (SetAllNotificationsAsReadRequest request,  SetNotificationsHandler handler, CancellationToken cancellationToken) =>
            {
                       var result = await handler.HandleAllNotificationsAsRead(request, cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
    }

    public static void SetSomeNotificationsAsRead(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/notification/logs/setsomenotificationsasread",
                   async (SetSomeNotificationsAsReadRequest request, SetNotificationsHandler handler, CancellationToken cancellationToken) =>
            {
                       var result = await handler.HandleSomeNotificationsAsRead(request, cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
    }
}
