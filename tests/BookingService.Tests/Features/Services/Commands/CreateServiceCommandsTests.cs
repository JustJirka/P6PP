using BookingService.API.Common.Exceptions;
using BookingService.API.Features.Services.Commands;
using BookingService.API.Features.Services.Models;
using BookingService.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using BookingService.API.Domain.Enums;
using BookingService.API.Domain.Models;

public class CreateServiceCommandHandlerTests
{
    private readonly DataContext _dbContext;
    private readonly CreateServiceCommandHandler _handler;

    public CreateServiceCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"ServiceTestDb_{System.Guid.NewGuid()}")
            .Options;

        _dbContext = new DataContext(options);
        _handler = new CreateServiceCommandHandler(_dbContext);
    }

    [Fact]
    public async Task Should_CreateService_When_RoomExists()
    {
        var room = new Room { Id = 1, Name = "Test Room", Capacity = 10, Status = RoomStatus.Available };
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.SaveChangesAsync();

        var serviceRequest = new CreateServiceRequest(
            Start: DateTime.Now.AddHours(1),
            End: DateTime.Now.AddHours(2),
            Price: 100,
            ServiceName: "Yoga Class",
            TrainerId: 1,
            RoomId: room.Id
        );

        var command = new CreateServiceCommand(serviceRequest);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(serviceRequest.ServiceName, result.ServiceName);
        Assert.Equal(room.Id, result.Id); 
        Assert.Equal(serviceRequest.Price, result.Price);
    }


    [Fact]
    public async Task Should_Throw_When_RoomDoesNotExist()
    {
        var serviceRequest = new CreateServiceRequest(
            Start: DateTime.Now.AddHours(1),       
            End: DateTime.Now.AddHours(2),         
            Price: 100,                            
            ServiceName: "Yoga Class",             
            TrainerId: 1,                          
            RoomId: 999                            
        );

        var command = new CreateServiceCommand(serviceRequest);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Room not found", ex.Message);
    }
}
