using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Application.DTOs;

namespace Analytics.Application.Services.Interface
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllBookings();
        Task<Booking?> GetBookingById(int id);
        Task<Booking> CreateBooking(BookingDto booking);
        Task<Booking?> DeleteBooking(int id);
    }
}