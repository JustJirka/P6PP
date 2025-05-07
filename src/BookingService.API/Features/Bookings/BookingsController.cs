using BookingService.API.Domain.Enums;
using BookingService.API.Features.Bookings.Commands;
using BookingService.API.Features.Bookings.Models;
using BookingService.API.Features.Bookings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Shared.Results;
using System.Net;

namespace BookingService.API.Features.Bookings;

[Route("api/[controller]")]
public sealed class BookingsController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// blablabla
    /// </summary>
    /// <param name="status"></param>
    /// <returns>List of bookings</returns>
    [HttpGet]
    //[Authorize]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResult<IList<BookingResponse>>))]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ApiResult<ProblemDetails>))]
    public async Task<IActionResult> Get(BookingStatus? status)
        => await ExecuteAsync(new GetBookingsQuery(status));

    /// <summary>
    /// Returns booking details by its ID.
    /// </summary>
    /// <param name="bookingId">The ID of the booking.</param>
    /// <returns>HTTP 200 with booking details, or HTTP 404 if the booking is not found.</returns>
    /// <response code="200">Booking found successfully.</response>
    /// <response code="404">Booking not found.</response>
    [HttpGet("{bookingId}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResult<BookingResponse>))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiResult<ProblemDetails>))]
    public async Task<IActionResult> GetDetail(int bookingId)
        => await ExecuteAsync(new GetBookingDetailQuery(bookingId));

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    /// <param name="booking">The booking data to create.</param>
    /// <returns>
    /// HTTP 201 if the booking is created successfully, or HTTP 400 if the request is invalid.
    /// </returns>
    /// <response code="201">Booking was created successfully.</response>
    /// <response code="400">Invalid input data. Returns validation errors.</response>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiResult<ValidationProblemDetails>))]
    public async Task<IActionResult> Create(CreateBookingRequest booking)
       => await ExecuteWithCreatedAtActionAsync(new CreateBookingCommand(booking), nameof(GetDetail), response => new { bookingId = response.Id });

    /// <summary>
    /// Deletes a booking by its ID.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to delete.</param>
    /// <returns>
    /// HTTP 204 if the booking was deleted, or HTTP 404 if the booking was not found.
    /// </returns>
    /// <response code="204">Booking was successfully deleted.</response>
    /// <response code="404">Booking not found.</response>
    [HttpDelete("{bookingId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiResult<ProblemDetails>))]
    public async Task<IActionResult> Delete(int bookingId)
        => await ExecuteWithNoContentAsync(new DeleteBookingCommand(bookingId));
}
