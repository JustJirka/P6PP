using BookingService.API.Domain.Models;
using BookingService.API.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;

namespace BookingService.API.Infrastructure;

public sealed class DataContext : DbContext
{
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Service> Services { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discount>().HasData(DiscountsInit.GetDiscounts());

        modelBuilder.Entity<Room>().HasData(RoomsInit.GetRooms());

        modelBuilder.Entity<Service>().HasData(ServicesInit.GetServices());

        modelBuilder.Entity<Booking>().HasData(BookingsInit.GetBookings());

        base.OnModelCreating(modelBuilder);
    }
}