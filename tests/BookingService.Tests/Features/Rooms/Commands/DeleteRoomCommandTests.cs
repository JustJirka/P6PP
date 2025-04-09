using BookingService.API.Common.Exceptions;
using BookingService.API.Features.Rooms.Commands;
using BookingService.API.Infrastructure;
using BookingService.API.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using BookingService.API.Domain.Enums;

public class DeleteRoomCommandHandlerTests
{
    private readonly DataContext _dbContext;
    private readonly DeleteRoomCommandHandler _handler;

    public DeleteRoomCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"RoomTestDb_{System.Guid.NewGuid()}")
            .Options;

        _dbContext = new DataContext(options);
        _handler = new DeleteRoomCommandHandler(_dbContext);
    }

    [Fact]
    public async Task Should_DeleteRoom_When_RoomExists_And_NotInUse()
    {
        var room = new Room { Id = 1, Name = "Test Room", Capacity = 10, Status = RoomStatus.Available };
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.SaveChangesAsync();

        var command = new DeleteRoomCommand(room.Id);

        await _handler.Handle(command, CancellationToken.None);

        var deletedRoom = await _dbContext.Rooms.SingleOrDefaultAsync(r => r.Id == room.Id);
        Assert.Null(deletedRoom);
    }

    [Fact]
    public async Task Should_Throw_When_RoomNotFound()
    {
        var nonExistentRoomId = 99;
        var command = new DeleteRoomCommand(nonExistentRoomId);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Room not found", ex.Message);
    }

    [Fact]
    public async Task Should_Throw_When_RoomIsInUseByServices()
    {
        var room = new Room { Id = 1, Name = "Test Room", Capacity = 10, Status = RoomStatus.Available };
        var service = new Service { Id = 1, Room = room, IsCancelled = false };

        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.Services.AddAsync(service);
        await _dbContext.SaveChangesAsync();

        var command = new DeleteRoomCommand(room.Id);

        var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Room is used in services.", ex.Message);
    }
}
