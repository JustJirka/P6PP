using BookingService.API.Features.Rooms.Commands;
using BookingService.API.Features.Rooms.Models;
using BookingService.API.Features.Rooms.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Shared.Results;
using System.Net;

namespace BookingService.API.Features.Rooms;

[Route("api/[controller]")]
//[Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Instructor))]
public sealed class RoomsController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Returns a list of available rooms.
    /// </summary>
    /// <returns>
    /// HTTP 200 with a list of rooms, or HTTP 401 if the user is not authorized.
    /// </returns>
    /// <response code="200">Rooms retrieved successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResult<IList<RoomResponse>>))]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ApiResult<ProblemDetails>))]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ApiResult<ProblemDetails>))]
    public async Task<IActionResult> Get()
        => await ExecuteAsync(new GetRoomsQuery());

    /// <summary>
    /// Returns the details of a specific room by its ID.
    /// </summary>
    /// <param name="roomId">The ID of the room.</param>
    /// <returns>
    /// HTTP 200 with room details, or HTTP 404 if the room is not found.
    /// </returns>
    /// <response code="200">Room found successfully.</response>
    /// <response code="404">Room not found.</response>
    [HttpGet("{roomId}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResult<RoomResponse>))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiResult<ProblemDetails>))]
    // [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ApiResult<ProblemDetails>))]
    public async Task<IActionResult> GetDetail(int roomId)
        => await ExecuteAsync(new GetRoomDetailQuery(roomId));

    /// <summary>
    /// Creates a new room.
    /// </summary>
    /// <param name="room">The room data to create.</param>
    /// <returns>
    /// HTTP 201 if the room is created successfully, or HTTP 400 if the request is invalid.
    /// </returns>
    /// <response code="201">Room was created successfully.</response>
    /// <response code="400">Invalid input data. Returns validation errors.</response>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiResult<ValidationProblemDetails>))]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ApiResult<ProblemDetails>))]
    public async Task<IActionResult> Create(RoomRequest room)
        => await ExecuteWithCreatedAtActionAsync(new CreateRoomCommand(room), nameof(GetDetail), response => new { roomId = response.Id });

    /// <summary>
    /// Updates an existing room.
    /// </summary>
    /// <param name="roomId">The ID of the room to update.</param>
    /// <param name="room">The updated room data.</param>
    /// <returns>
    /// HTTP 200 with the updated room details, or HTTP 400 if the request is invalid.
    /// </returns>
    /// <response code="200">Room updated successfully.</response>
    /// <response code="400">Invalid input data. Returns validation errors.</response>
    [HttpPut("{roomId}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResult<RoomResponse>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiResult<ValidationProblemDetails>))]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ApiResult<ProblemDetails>))]
    public async Task<IActionResult> Update(int roomId, RoomRequest room)
        => await ExecuteAsync(new UpdateRoomCommand(roomId, room));

    /// <summary>
    /// Deletes a room by its ID.
    /// </summary>
    /// <param name="roomId">The ID of the room to delete.</param>
    /// <returns>
    /// HTTP 204 if the room is deleted successfully, or HTTP 404 if the room is not found.
    /// </returns>
    /// <response code="204">Room deleted successfully.</response>
    /// <response code="404">Room not found.</response>
    /// <response code="400">Invalid input data. Returns validation errors.</response>
    [HttpDelete("{roomId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiResult<ProblemDetails>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiResult<ValidationProblemDetails>))]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ApiResult<ProblemDetails>))]
    public async Task<IActionResult> Delete(int roomId)
        => await ExecuteWithNoContentAsync(new DeleteRoomCommand(roomId));
}
