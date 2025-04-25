using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NotificationService.API.Persistence;
using NotificationService.API.Persistence.Entities.DB;
using NotificationService.API.Persistence.Entities;
using NotificationService.API.Persistence.Entities.DB.Models;
using NotificationService.API.Logging; // <-- Přidáno pro FileLogger
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared.Results;

namespace NotificationService.API.Services
{
    public class BookingAppService
    {
        private readonly NetworkHttpClient _httpClient;
        private readonly NotificationDbContext _notificationDbContext;
        private readonly UserAppService _userAppService;
        private readonly TemplateAppService _templateAppService;
        private readonly MailAppService _mailAppService;

        public BookingAppService(NetworkHttpClient httpClient, NotificationDbContext notificationDbContext,
            UserAppService userAppService,
            TemplateAppService templateAppService, MailAppService mailAppService)
        {
            _httpClient = httpClient;
            _notificationDbContext = notificationDbContext;
            _userAppService = userAppService;
            _templateAppService = templateAppService;
            _mailAppService = mailAppService;
        }

        public async Task<BookingResponse?> GetBookingByIdAsync(int id)
        {
            ApiResult<BookingResponse>? response = null;
            try
            {
               
                response = await _httpClient.GetAsync<BookingResponse>("http://host.docker.internal:8080/api/Bookings/" + id);
                Console.WriteLine(response.Data);
            }
            catch (Exception e)
            {
                // Logování chyby
                await FileLogger.LogException(e, $"Error while fetching booking with ID {id}");
            }
            return response?.Data;
        }

        public async Task<ServiceResponse?> GetServiceByIdAsync(int serviceId)
        {
            ApiResult<ServiceResponse>? response = null;
            try
            {
                response = await _httpClient.GetAsync<ServiceResponse>("http://host.docker.internal:8080/api/Services/" + serviceId);
                Console.WriteLine(response.Data);

            }
            catch (Exception e)
            {
                // Logování chyby
                await FileLogger.LogException(e, $"Error while fetching service with ID {serviceId}");
            }

            return response?.Data;
        }

        public async Task<ServiceResponse?> GetServiceByBookingIdAsync(int bookingId)
        {
            var booking = await GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                return null;
            }

            var service = await GetServiceByIdAsync(booking.serviceId);
            return service;
        }

        public async Task SaveToTimer(int bookingId, int userId)
        {
            var booking = await GetBookingByIdAsync(bookingId);

            if (booking == null)
            {
                // Logování chyby
                FileLogger.LogError($"Booking with ID {bookingId} not found.");
                return;
            }

            var service = await GetServiceByIdAsync(booking.serviceId);
            if (service == null)
            {
                // Logování chyby
                FileLogger.LogError($"Service with ID {booking.serviceId} not found for booking {bookingId}.");
                return;
            }

            var bookingTimer = new Booking()
            {
                BookingId = booking.id,
                Start = service.start,
            };

            if (await _notificationDbContext.Bookings
                    .AnyAsync(b => b.BookingId == bookingTimer.BookingId))
            {
                return;
            }
            if (bookingTimer.Start < DateTime.UtcNow.AddHours(30))
            {
                bookingTimer.Notice24H = true;
            }
            var bookingTimerDb = await _notificationDbContext.Bookings.AddAsync(bookingTimer);
        }

        public async Task DeleteTimer(int bookingId)
        {
            var booking = await _notificationDbContext.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking != null)
            {
                _notificationDbContext.Bookings.Remove(booking);
                await _notificationDbContext.SaveChangesAsync();
            }
        }

        public async Task SendReminder24HourBefore()
        {
            var bookings = await _notificationDbContext.Bookings
                .Where(b => b.Start <= DateTime.UtcNow.AddHours(24) && b.Notice24H == false)
                .ToListAsync();
            foreach (var booking in bookings)
            {
                var success = await SendReminderEmail(booking);
                if (success)
                {
                    booking.Notice24H = true;
                    _notificationDbContext.Bookings.Update(booking);
                    await _notificationDbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<bool> SendReminderEmail(Booking bookingLocal)
        {
            var booking = await GetBookingByIdAsync(bookingLocal.BookingId);

            if (booking == null)
            {
                // Logování chyby
                FileLogger.LogError($"Booking with ID {bookingLocal.BookingId} not found.");
                return false;
            }

            if (booking.status == "Cancelled")
            {
                return true;
            }

            var service = await GetServiceByIdAsync(booking.serviceId);

            if (service == null)
            {
                // Logování chyby
                FileLogger.LogError($"Service with ID {booking.serviceId} not found for booking {bookingLocal.BookingId}.");
                return false;
            }

            var user = await _userAppService.GetUserByIdAsync(booking.userId);

            if (user == null)
            {
                // Logování chyby
                FileLogger.LogError($"User with ID {booking.userId} not found.");
                return false;
            }

            // Změna jména šablony. Čekání na vytvoření.
            var template = await _templateAppService.GetTemplateAsync("BookingReminder");

            template.Text = template.Text.Replace("{name}", user.FirstName + " " + user.LastName);
            template.Text = template.Text.Replace("{eventname}", service.serviceName);
            template.Text = template.Text.Replace("{datetime}", service.start.ToString());

            var emailArgs = new EmailArgs
            {
                Address = new List<string> { user.Email },
                Subject = template.Subject,
                Body = template.Text
            };

            try
            {
                await _mailAppService.SendEmailAsync(emailArgs);
            }
            catch (Exception e)
            {
                // Logování chyby
                await FileLogger.LogException(e, "Failed to send reminder email.");
                return false;
            }

            return true;
        }
    }
}
