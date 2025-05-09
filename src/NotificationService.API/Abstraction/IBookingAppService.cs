using NotificationService.API.Persistence.Entities.DB.Models;
using NotificationService.API.Persistence.Entities;
using ReservationSystem.Shared.Results;

namespace NotificationService.API.Services
{
    public interface IBookingAppService
    {
        Task<BookingResponse?> GetBookingByIdAsync(int id);
        Task<ServiceResponse?> GetServiceByIdAsync(int serviceId);
        Task<ServiceResponse?> GetServiceByBookingIdAsync(int bookingId);
        Task SaveToTimer(int bookingId, int userId);
        Task DeleteTimer(int bookingId);
        Task SendReminder24HourBefore();
        Task<bool> SendReminderEmail(Booking bookingLocal);
    }
}
