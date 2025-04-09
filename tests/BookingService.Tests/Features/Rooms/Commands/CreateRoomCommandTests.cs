using BookingService.API.Features.Rooms.Commands;
using BookingService.API.Features.Rooms.Models;
using BookingService.API.Infrastructure;
using BookingService.API.Domain.Enums;
using BookingService.API.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CreateRoomCommandHandlerTests
{
    private readonly DataContext _dbContext;
    private readonly CreateRoomCommandHandler _handler;

    public CreateRoomCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase($"RoomDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new DataContext(options);
        _handler = new CreateRoomCommandHandler(_dbContext);
    }

    [Fact]
    public async Task Should_CreateRoom_When_RequestIsValid()
    {
        var request = new RoomRequest("Main Hall", 50, RoomStatus.Available);

        var command = new CreateRoomCommand(request);
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Main Hall", result.Name);
        Assert.Equal(50, result.Capacity);
        Assert.Equal(RoomStatus.Available, result.Status);
    }

    //[Fact]
    //public async Task Should_Throw_When_RoomName_Is_Empty()
    //{
    //    var request = new RoomRequest(string.Empty, 50, RoomStatus.Available);
    //    var command = new CreateRoomCommand(request);

    //    var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
    //        await _handler.Handle(command, CancellationToken.None));

    //    Assert.Equal("Room name cannot be empty", ex.Message);
    //}

    //[Fact]
    //public async Task Should_Throw_When_RoomCapacity_Is_Negative()
    //{
    //    var request = new RoomRequest("Small Room", -10, RoomStatus.Available);
    //    var command = new CreateRoomCommand(request);

    //    // Act & Assert
    //    var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
    //        await _handler.Handle(command, CancellationToken.None));

    //    Assert.Equal("Room capacity must be a positive integer", ex.Message);
    //}

    //[Fact]
    //public async Task Should_Throw_When_RoomStatus_Is_Invalid()
    //{
    //    var request = new RoomRequest("Invalid Room", 50, (RoomStatus)999);
    //    var command = new CreateRoomCommand(request);

    //    var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
    //        await _handler.Handle(command, CancellationToken.None));

    //    Assert.Equal("Invalid room status", ex.Message);
    //}
}
