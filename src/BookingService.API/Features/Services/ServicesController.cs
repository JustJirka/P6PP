using System.Net;
using BookingService.API.Features.Rooms.Commands;
using BookingService.API.Features.Rooms.Models;
using BookingService.API.Features.Services.Commands;
using BookingService.API.Features.Services.Models;
using BookingService.API.Features.Services.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Features.Services;

[Route("api/[controller]")]
public sealed class ServicesController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IList<ServiceResponse>))]
    public async Task<IActionResult> GetAll()
        => await ExecuteAsync(new GetServicesQuery());

    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IList<ServiceResponse>))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [HttpGet("trainer/{trainerId}")]
    public async Task<IActionResult> Get(int trainerId)
        => await ExecuteAsync(new GetServicesQuery(trainerId));

    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetDetail(int serviceId)
        => await ExecuteAsync(new GetServiceDetailQuery(serviceId));

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Create(CreateServiceRequest service)
       => await ExecuteWithCreatedAtActionAsync(new CreateServiceCommand(service), nameof(GetDetail), response => new { serviceId = response.Id });


    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [HttpDelete("{serviceId}")]
    public async Task<IActionResult> Delete(int serviceId)
        => await ExecuteWithNoContentAsync(new DeleteServiceCommand(serviceId));

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RoomResponse))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Update(UpdateServiceRequest service)
        => await ExecuteAsync(new UpdateServiceCommand(service));
}
