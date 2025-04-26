using Analytics.Application.Services.Interface;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;
using Analytics.Application.DTOs;
using Analytics.Domain.Enums;

namespace Analytics.Application.Services
{
    public class BookingService(IBookingRepository bookingRepository) : IBookingService
    {
        public async Task<List<Booking>> GetAllBookings()
        {
            return await bookingRepository.GetAll();
        }

        public async Task<Booking> GetBookingById(int id)
        {
            return await bookingRepository.GetById(id);
        }

        public async Task<Booking> CreateBooking(BookingDto booking)
        {
            if (string.IsNullOrEmpty(booking.bookingDate))
                throw new ArgumentNullException(nameof(booking.bookingDate), "Booking date cannot be null or empty.");

            var newBooking = new Booking()
            {
                Id = booking.id,
                BookingDate = DateTime.Parse(booking.bookingDate),
                Status = Enum.Parse<BookingStatus>(booking.status, ignoreCase: true), //Confirmed 0, Pending 1, Cancelled 2
                UserId = booking.userId,
                ServiceId = booking.serviceId
            };

            await bookingRepository.Create(newBooking);
            return newBooking;
        }

        public async Task<Booking> DeleteBooking(int id)
        {
            return await bookingRepository.Delete(id);
        }
    }
}
