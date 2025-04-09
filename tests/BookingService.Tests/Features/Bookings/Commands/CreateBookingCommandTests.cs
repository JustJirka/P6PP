using BookingService.API.Domain.Enums;
using BookingService.API.Features.Bookings.Commands;
using BookingService.API.Features.Bookings.Models;
using BookingService.API.Infrastructure;
using BookingService.API.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using BookingService.API.Domain.Models;

public class CreateBookingCommandHandlerTests
{
    private readonly DataContext _dbContext;
    private readonly CreateBookingCommandHandler _handler;

    public CreateBookingCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"BookingTestDb_{System.Guid.NewGuid()}")
            .Options;

        _dbContext = new DataContext(options);
        _handler = new CreateBookingCommandHandler(_dbContext);
    }

    [Fact]
    public async Task Should_CreateBooking_When_Valid()
    {
        var room = new Room { Id = 1,Name = "Test Room", Capacity = 5 };
        var service = new Service
        {
            Id = 1,
            Room = room,
            IsCancelled = false,
            Users = new List<int>()
        };

        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.Services.AddAsync(service);
        await _dbContext.SaveChangesAsync();

        var request = new CreateBookingRequest(1);

        var command = new CreateBookingCommand(request)
        {
            UserId = 123
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.ServiceId);
        Assert.Equal(BookingStatus.Pending, result.Status);
    }

    [Fact]
    public async Task Should_Throw_When_ServiceNotFound()
    {
        var command = new CreateBookingCommand(new CreateBookingRequest(99))
        {
            UserId = 123
        };

        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Throw_When_ServiceIsCancelled()
    {
        var room = new Room { Id = 1,Name = "Canceled Room", Capacity = 5 };
        var service = new Service
        {
            Id = 2,
            Room = room,
            IsCancelled = true,
            Users = new List<int>()
        };

        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.Services.AddAsync(service);
        await _dbContext.SaveChangesAsync();

        var command = new CreateBookingCommand(new CreateBookingRequest(2))
        {
            UserId = 123
        };

        var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Service is cancelled", ex.Message);
    }

    [Fact]
    public async Task Should_Throw_When_UserAlreadyRegistered()
    {
        var room = new Room { Id = 1, Name = "Registered Room", Capacity = 5 };
        var service = new Service
        {
            Id = 3,
            Room = room,
            IsCancelled = false,
            Users = new List<int> { 123 }
        };

        var booking = new Booking
        {
            Id = 1,
            ServiceId = 3,
            UserId = 123,
            BookingDate = System.DateTime.UtcNow,
            Status = BookingStatus.Pending
        };

        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.Services.AddAsync(service);
        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();

        var command = new CreateBookingCommand(new CreateBookingRequest(3))
        {
            UserId = 123
        };

        var ex = await Assert.ThrowsAsync<ValidationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("User already registered on this service", ex.Message);
    }
}
