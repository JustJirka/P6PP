using BookingService.API.Common.Exceptions;
using BookingService.API.Features.Services.Models;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System.Threading;
using BookingService.API.Domain.Enums;
using BookingService.API.Domain.Models;
using BookingService.API.Features.Services.Commands;

public class UpdateServiceCommandHandlerTests
{
    private readonly DataContext _dbContext;
    private readonly UpdateServiceCommandHandler _handler;

    public UpdateServiceCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"ServiceTestDb_{System.Guid.NewGuid()}")
            .Options;

        _dbContext = new DataContext(options);
        _handler = new UpdateServiceCommandHandler(_dbContext);
    }

    [Fact]
    public async Task Should_UpdateService_When_ServiceAndRoomExist()
    {
        var room = new Room { Id = 1, Name = "Test Room", Capacity = 10, Status = RoomStatus.Available };
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.SaveChangesAsync();

        var service = new Service { Id = 1, ServiceName = "Yoga Class", RoomId = room.Id };
        await _dbContext.Services.AddAsync(service);
        await _dbContext.SaveChangesAsync();

        var updateRequest = new UpdateServiceRequest(
            Id: service.Id,
            Start: DateTime.Now.AddHours(1),
            End: DateTime.Now.AddHours(2),
            ServiceName: "Updated Yoga Class",
            RoomId: room.Id
        );

        var command = new UpdateServiceCommand(updateRequest);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(updateRequest.ServiceName, result.ServiceName);
        Assert.Equal(updateRequest.RoomId, result.Id);
    }

    [Fact]
    public async Task Should_Throw_When_ServiceDoesNotExist()
    {
        var updateRequest = new UpdateServiceRequest(
            Id: 999,
            ServiceName: "Updated Yoga Class",
            RoomId: 1,
            Start: DateTime.Now.AddHours(1),
            End: DateTime.Now.AddHours(2)
        );

        var command = new UpdateServiceCommand(updateRequest);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Service not found", ex.Message);
    }

    [Fact]
    public async Task Should_Throw_When_RoomDoesNotExist()
    {
        var room = new Room { Id = 1, Name = "Test Room", Capacity = 10, Status = RoomStatus.Available };
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.SaveChangesAsync();

        var service = new Service { Id = 1, ServiceName = "Yoga Class", RoomId = room.Id };
        await _dbContext.Services.AddAsync(service);
        await _dbContext.SaveChangesAsync();

        var updateRequest = new UpdateServiceRequest(
            Id: service.Id,
            ServiceName: "Updated Yoga Class",
            RoomId: 999,
            Start: DateTime.Now.AddHours(1),
            End: DateTime.Now.AddHours(2)
        );

        var command = new UpdateServiceCommand(updateRequest);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Room not found", ex.Message);
    }
}
