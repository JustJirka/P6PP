using BookingService.API.Domain.Enums;
using BookingService.API.Features.Bookings.Commands;
using BookingService.API.Features.Bookings.Models;
using BookingService.API.Features.Bookings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookingService.API.Features.Bookings;

[Route("api/[controller]")]
public sealed class BookingsController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpGet]
    //[Authorize]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IList<BookingResponse>))]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Get(BookingStatus? status)
        => await ExecuteAsync(new GetBookingsQuery(status));

    [HttpGet("{bookingId}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BookingResponse))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetDetail(int bookingId)
        => await ExecuteAsync(new GetBookingDetailQuery(bookingId));

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Create(CreateBookingRequest booking)
       => await ExecuteWithCreatedAtActionAsync(new CreateBookingCommand(booking), nameof(GetDetail), response => new { bookingId = response.Id });

    [HttpDelete("{bookingId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(int bookingId)
        => await ExecuteWithNoContentAsync(new DeleteBookingCommand(bookingId));
}
