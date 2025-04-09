using BookingService.API.Features.Rooms.Commands;
using BookingService.API.Features.Rooms.Models;
using BookingService.API.Features.Rooms.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookingService.API.Features.Rooms;

[Route("api/[controller]")]
//[Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Instructor))]
public sealed class RoomsController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IList<RoomResponse>))]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ProblemDetails))]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Get()
        => await ExecuteAsync(new GetRoomsQuery());

    [HttpGet("{roomId}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RoomResponse))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    // [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetDetail(int roomId)
        => await ExecuteAsync(new GetRoomDetailQuery(roomId));

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ValidationProblemDetails))]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Create(RoomRequest room)
        => await ExecuteWithCreatedAtActionAsync(new CreateRoomCommand(room), nameof(GetDetail), response => response.Id);

    [HttpPut("{roomId}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RoomResponse))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ValidationProblemDetails))]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Update(int roomId, RoomRequest room)
        => await ExecuteAsync(new UpdateRoomCommand(roomId, room));

    [HttpDelete("{roomId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ValidationProblemDetails))]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(int roomId)
        => await ExecuteWithNoContentAsync(new DeleteRoomCommand(roomId));
}
