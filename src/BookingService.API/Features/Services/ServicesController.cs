using BookingService.API.Features.Rooms.Models;
using BookingService.API.Features.Services.Commands;
using BookingService.API.Features.Services.Models;
using BookingService.API.Features.Services.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Shared.Results;
using System.Net;

namespace BookingService.API.Features.Services;

[Route("api/[controller]")]
public sealed class ServicesController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Returns a list of all available services.
    /// </summary>
    /// <returns>
    /// HTTP 200 with a list of services, or HTTP 404 if no services are found.
    /// </returns>
    /// <response code="200">Services retrieved successfully.</response>
    /// <response code="404">No services found.</response>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResult<IList<ServiceResponse>>))]
    public async Task<IActionResult> GetAll()
        => await ExecuteAsync(new GetServicesQuery());

    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResult<IList<ServiceResponse>>))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiResult<ProblemDetails>))]

    /// <summary>
    /// Returns a list of services offered by a specific trainer.
    /// </summary>
    /// <param name="trainerId">The ID of the trainer.</param>
    /// <returns>
    /// HTTP 200 with a list of services, or HTTP 404 if no services are found for the trainer.
    /// </returns>
    /// <response code="200">Services retrieved successfully for the specified trainer.</response>
    /// <response code="404">No services found for the specified trainer.</response>
    [HttpGet("trainer/{trainerId}")]
    public async Task<IActionResult> Get(int trainerId)
        => await ExecuteAsync(new GetServicesQuery(trainerId));

    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResult<ServiceResponse>))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiResult<ProblemDetails>))]

    /// <summary>
    /// Returns the details of a specific service by its ID.
    /// </summary>
    /// <param name="serviceId">The ID of the service.</param>
    /// <returns>
    /// HTTP 200 with the service details, or HTTP 404 if the service is not found.
    /// </returns>
    /// <response code="200">Service details retrieved successfully.</response>
    /// <response code="404">Service not found.</response>
    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetDetail(int serviceId)
        => await ExecuteAsync(new GetServiceDetailQuery(serviceId));

    /// <summary>
    /// Creates a new service.
    /// </summary>
    /// <param name="service">The data for the new service to be created.</param>
    /// <returns>
    /// HTTP 201 if the service is created successfully, or HTTP 400 if the request is invalid.
    /// </returns>
    /// <response code="201">Service created successfully.</response>
    /// <response code="400">Invalid input data. Returns validation errors.</response>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiResult<ValidationProblemDetails>))]
    public async Task<IActionResult> Create(CreateServiceRequest service)
       => await ExecuteWithCreatedAtActionAsync(new CreateServiceCommand(service), nameof(GetDetail), response => new { serviceId = response.Id });


    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiResult<ProblemDetails>))]

    /// <summary>
    /// Deletes a service by its ID.
    /// </summary>
    /// <param name="serviceId">The ID of the service to delete.</param>
    /// <returns>
    /// HTTP 204 if the service is deleted successfully, or HTTP 404 if the service is not found.
    /// </returns>
    /// <response code="204">Service deleted successfully.</response>
    /// <response code="404">Service not found.</response>
    [HttpDelete("{serviceId}")]
    public async Task<IActionResult> Delete(int serviceId)
        => await ExecuteWithNoContentAsync(new DeleteServiceCommand(serviceId));

    /// <summary>
    /// Updates an existing service.
    /// </summary>
    /// <param name="service">The updated data for the service.</param>
    /// <returns>
    /// HTTP 200 with the updated service details, HTTP 400 if the request is invalid, or HTTP 404 if the service is not found.
    /// </returns>
    /// <response code="200">Service updated successfully.</response>
    /// <response code="400">Invalid input data. Returns validation errors.</response>
    /// <response code="404">Service not found.</response>
    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResult<RoomResponse>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiResult<ValidationProblemDetails>))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiResult<ValidationProblemDetails>))]
    public async Task<IActionResult> Update(UpdateServiceRequest service)
        => await ExecuteAsync(new UpdateServiceCommand(service));
}
