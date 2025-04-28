using Analytics.Application.Services.Interface;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;
using Analytics.Application.DTOs;
using Analytics.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<Booking>> GetAllBookings()
        {
            return await _bookingRepository.GetAll();
        }

        public async Task<Booking?> GetBookingById(int id)
        {
            return await _bookingRepository.GetById(id);
        }

        public async Task<Booking> CreateBooking(BookingDto booking)
        {
            if (string.IsNullOrEmpty(booking.bookingDate))
                throw new ArgumentNullException(nameof(booking.bookingDate), "Booking date cannot be null or empty.");

            var newBooking = new Booking()
            {
                Id = booking.id,
                BookingDate = DateTime.Parse(booking.bookingDate),
                Status = Enum.Parse<BookingStatus>(booking.status, ignoreCase: true),
                UserId = booking.userId,
                ServiceId = booking.serviceId
                // Service property will be populated by repository
            };

            await _bookingRepository.Create(newBooking);
            return newBooking;
        }

        public async Task<Booking?> DeleteBooking(int id)
        {
            return await _bookingRepository.Delete(id);
        }
    }
}