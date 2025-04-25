using BookingService.API.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Shared.Results;

namespace BookingService.API.Features;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private readonly IMediator _mediator;

    protected ApiControllerBase(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected async Task<IActionResult> ExecuteAsync<TResponse>(IRequest<TResponse> request) where TResponse : class
        => await HandleRequestAsync(async () =>
        {
            var response = await _mediator.Send(request);
            return Ok(new ApiResult<TResponse>(response, true));
        });

    protected async Task<IActionResult> ExecuteWithNoContentAsync(IRequest request)
        => await HandleRequestAsync(async () =>
        {
            await _mediator.Send(request);
            return NoContent();
        });

    protected async Task<IActionResult> ExecuteWithCreatedAtActionAsync<TResponse>(IRequest<TResponse> request, string actionName, Func<TResponse, object> routeValuesFunc) where TResponse : class
    {
        return await HandleRequestAsync(async () =>
        {
            TResponse response = await _mediator.Send(request);

            var routeValues = routeValuesFunc(response);

            return CreatedAtAction(actionName, routeValues, new ApiResult<TResponse>(response, true));
        });
    }

    private async Task<IActionResult> HandleRequestAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ApiResult<object>(null, false, ex.Message));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResult<object>(null, false, ex.Message));
        }
        catch (AuthException ex)
        {
            return Unauthorized(new ApiResult<object>(null, false, ex.Message));
        }
        catch (Exception)
        {
            // TODO: Log exception
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult<object>(null, false, "An unexpected error happened."));
        }
    }
}