using BookingService.API.Common.Exceptions;
using BookingService.API.Features.Rooms.Commands;
using BookingService.API.Features.Rooms.Models;
using BookingService.API.Infrastructure;
using BookingService.API.Domain.Models;
using BookingService.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UpdateRoomCommandHandlerTests
{
    private readonly DataContext _dbContext;
    private readonly UpdateRoomCommandHandler _handler;

    public UpdateRoomCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"RoomTestDb_{System.Guid.NewGuid()}")
            .Options;

        _dbContext = new DataContext(options);
        _handler = new UpdateRoomCommandHandler(_dbContext);
    }

    [Fact]
    public async Task Should_UpdateRoom_When_RoomExists()
    {
        var room = new Room { Id = 1, Name = "Old Room", Capacity = 10, Status = RoomStatus.Available };
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.SaveChangesAsync();

        var roomRequest = new RoomRequest("Updated Room", 20, RoomStatus.Occupied);
        var command = new UpdateRoomCommand(room.Id, roomRequest);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Updated Room", result.Name);
        Assert.Equal(20, result.Capacity);
        Assert.Equal(RoomStatus.Occupied, result.Status);
    }

    [Fact]
    public async Task Should_Throw_When_RoomNotFound()
    {
        var nonExistentRoomId = 99;
        var roomRequest = new RoomRequest("Test Room", 30, RoomStatus.Available);
        var command = new UpdateRoomCommand(nonExistentRoomId, roomRequest);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Room not found", ex.Message);
    }

    //[Fact]
    //public async Task Should_Throw_When_RoomName_Is_Empty()
    //{
    //    var room = new Room { Id = 1, Name = "Test Room", Capacity = 10, Status = RoomStatus.Available };
    //    await _dbContext.Rooms.AddAsync(room);
    //    await _dbContext.SaveChangesAsync();

    //    var roomRequest = new RoomRequest("", 10, RoomStatus.Available); // Empty name
    //    var command = new UpdateRoomCommand(room.Id, roomRequest);

    //    var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
    //        await _handler.Handle(command, CancellationToken.None));

    //    Assert.Equal("Room name cannot be empty", ex.Message);
    //}

    //[Fact]
    //public async Task Should_Throw_When_RoomCapacity_Is_Zero()
    //{
    //    
    //    var room = new Room { Id = 1, Name = "Test Room", Capacity = 10, Status = RoomStatus.Available };
    //    await _dbContext.Rooms.AddAsync(room);
    //    await _dbContext.SaveChangesAsync();

    //    var invalidRequest = new RoomRequest("Invalid Room", 0, RoomStatus.Available); // Capacity is 0
    //    var command = new UpdateRoomCommand(room.Id, invalidRequest);

    //   
    //    var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
    //        await _handler.Handle(command, CancellationToken.None));

    //    Assert.Equal("Room capacity must be greater than 0", ex.Message);
    //}

    //[Fact]
    //public async Task Should_Throw_When_RoomCapacity_Is_Negative()
    //{
    //    
    //    var room = new Room { Id = 1, Name = "Test Room", Capacity = 10, Status = RoomStatus.Available };
    //    await _dbContext.Rooms.AddAsync(room);
    //    await _dbContext.SaveChangesAsync();

    //    var invalidRequest = new RoomRequest("Invalid Room", -5, RoomStatus.Available); // Negative capacity
    //    var command = new UpdateRoomCommand(room.Id, invalidRequest);

    //    
    //    var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
    //        await _handler.Handle(command, CancellationToken.None));

    //    Assert.Equal("Room capacity must be greater than 0", ex.Message);
    //}
}
