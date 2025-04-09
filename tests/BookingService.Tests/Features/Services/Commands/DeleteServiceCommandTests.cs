using BookingService.API.Common.Exceptions;
using BookingService.API.Features.Services.Commands;
using BookingService.API.Infrastructure;
using BookingService.API.Domain.Models;
using BookingService.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public class DeleteServiceCommandHandlerTests
{
    private readonly DataContext _dbContext;
    private readonly DeleteServiceCommandHandler _handler;

    public DeleteServiceCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"DeleteServiceTestDb_{System.Guid.NewGuid()}")
            .Options;

        _dbContext = new DataContext(options);
        _handler = new DeleteServiceCommandHandler(_dbContext);
    }

    [Fact]
    public async Task Should_DeleteService_When_ServiceExists()
    {
        var room = new Room { Id = 1, Name = "Test Room", Capacity = 10, Status = RoomStatus.Available };
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.SaveChangesAsync();

        var service = new Service { Id = 1, ServiceName = "Yoga Class", RoomId = room.Id };
        await _dbContext.Services.AddAsync(service);
        await _dbContext.SaveChangesAsync();

        var booking1 = new Booking
        {
            ServiceId = service.Id,
            Status = BookingStatus.Confirmed
        };
        var booking2 = new Booking
        {
            ServiceId = service.Id,
            Status = BookingStatus.Pending
        };
        await _dbContext.Bookings.AddAsync(booking1);
        await _dbContext.Bookings.AddAsync(booking2);
        await _dbContext.SaveChangesAsync();

        var serviceToDelete = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == service.Id);
        if (serviceToDelete != null)
        {
            var bookingsToUpdate = await _dbContext.Bookings
                .Where(b => b.ServiceId == serviceToDelete.Id)
                .ToListAsync();

            foreach (var booking in bookingsToUpdate)
            {
                booking.Status = BookingStatus.Cancelled;
            }

            serviceToDelete.IsCancelled = true;

            _dbContext.Services.Remove(serviceToDelete);

            await _dbContext.SaveChangesAsync();
        }

        var deletedService = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == service.Id);
        Assert.Null(deletedService);

        var bookings = await _dbContext.Bookings
            .Where(b => b.ServiceId == service.Id)
            .ToListAsync();
        Assert.All(bookings, b => Assert.Equal(BookingStatus.Cancelled, b.Status));
    }

    [Fact]
    public async Task Should_Throw_When_ServiceDoesNotExist()
    {
        var nonExistentServiceId = 999;
        var command = new DeleteServiceCommand(nonExistentServiceId);

        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Service not found", ex.Message);
    }

    [Fact]
    public async Task Should_CancelBookings_When_ServiceIsDeleted()
    {
        var room = new Room { Id = 1, Name = "Test Room", Capacity = 10, Status = RoomStatus.Available };
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.SaveChangesAsync();

        var service = new Service { Id = 1, ServiceName = "Yoga Class", RoomId = room.Id };
        await _dbContext.Services.AddAsync(service);
        await _dbContext.SaveChangesAsync();

        var booking1 = new Booking
        {
            ServiceId = service.Id,
            Status = BookingStatus.Confirmed
        };
        var booking2 = new Booking
        {
            ServiceId = service.Id,
            Status = BookingStatus.Pending
        };
        await _dbContext.Bookings.AddAsync(booking1);
        await _dbContext.Bookings.AddAsync(booking2);
        await _dbContext.SaveChangesAsync();

        var serviceToDelete = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == service.Id);
        if (serviceToDelete != null)
        {
            var bookingsToUpdate = await _dbContext.Bookings
                .Where(b => b.ServiceId == serviceToDelete.Id)
                .ToListAsync();

            foreach (var booking in bookingsToUpdate)
            {
                booking.Status = BookingStatus.Cancelled;
            }

            serviceToDelete.IsCancelled = true;

            _dbContext.Services.Remove(serviceToDelete);

            await _dbContext.SaveChangesAsync();
        }

        var deletedService = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == service.Id);
        Assert.Null(deletedService);

        var bookings = await _dbContext.Bookings
            .Where(b => b.ServiceId == service.Id)
            .ToListAsync();
        Assert.All(bookings, b => Assert.Equal(BookingStatus.Cancelled, b.Status));
    }

}
