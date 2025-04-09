using BookingService.API.Common.Exceptions;
using BookingService.API.Domain.Enums;
using BookingService.API.Domain.Models;
using BookingService.API.Features.Bookings.Commands;
using BookingService.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DeleteBookingCommandHandlerTests
{
    private readonly DataContext _dbContext;
    private readonly DeleteBookingCommandHandler _handler;

    public DeleteBookingCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase($"DeleteBookingTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new DataContext(options);
        _handler = new DeleteBookingCommandHandler(_dbContext);
    }

    [Fact]
    public async Task Should_DeleteBooking_When_Exists()
    {
        var room = new Room { Id = 1, Name = "DeleteRoom", Capacity = 5 };
        var service = new Service
        {
            Id = 1,
            Room = room,
            Users = new List<int> { 123 }
        };
        var booking = new Booking
        {
            Id = 1,
            ServiceId = 1,
            UserId = 123,
            BookingDate = DateTime.UtcNow,
            Status = BookingStatus.Pending
        };

        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.Services.AddAsync(service);
        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();

        var command = new DeleteBookingCommand(1);
        await _handler.Handle(command, CancellationToken.None);

        var deletedBooking = await _dbContext.Bookings.FindAsync(1);
        var updatedService = await _dbContext.Services.FindAsync(1);

        Assert.Null(deletedBooking);
        Assert.DoesNotContain(123, updatedService!.Users);
    }

    [Fact]
    public async Task Should_Throw_When_BookingNotFound()
    {
        var command = new DeleteBookingCommand(999); 

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Booking not found", ex.Message);
    }

    [Fact]
    public async Task Should_Throw_When_ServiceNotFound()
    {
        var booking = new Booking
        {
            Id = 2,
            ServiceId = 999, 
            UserId = 456,
            BookingDate = DateTime.UtcNow,
            Status = BookingStatus.Pending
        };

        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();

        var command = new DeleteBookingCommand(2);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Service not found", ex.Message);
    }
}
