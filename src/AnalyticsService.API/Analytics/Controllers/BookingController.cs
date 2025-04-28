using Analytics.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Analytics.Application.DTOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Analytics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IDatabaseSyncService _databaseSyncService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IBookingService bookingService, 
                                 IDatabaseSyncService databaseSyncService, 
                                 ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _databaseSyncService = databaseSyncService;
            _logger = logger;
        }        

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookings();
            
            _logger.LogInformation($"Retrieved {bookings.Count} bookings");
            
            var bookingDtos = bookings.Select(b => new BookingDto {
                id = b.Id,
                userId = b.UserId,
                serviceId = b.ServiceId,
                bookingDate = b.BookingDate.ToString("o"),
                status = b.Status.ToString(),
                service = b.Service != null ? new ServiceDto {
                    trainerId = b.Service.TrainerId,
                    serviceName = b.Service.ServiceName ?? string.Empty,
                    start = b.Service.Start.ToString("o"),
                    end = b.Service.End.ToString("o"),
                    roomId = b.Service.RoomId,
                    room = b.Service.Room != null ? new RoomDto {
                        name = b.Service.Room.Name,
                        capacity = b.Service.Room.Capacity
                    } : null,
                    users = b.Service.Users ?? new List<int>()
                } : null
            }).ToList();
            
            foreach (var dto in bookingDtos)
            {
                _logger.LogInformation($"Mapped booking {dto.id}: Has Service: {dto.service != null}, " +
                                    $"Service has Room: {dto.service?.room != null}");
            }
            
            return Ok(bookingDtos);
        }

        // GET: api/Bookings/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingDto>> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingById(id);
            if (booking == null)
            {
                return NotFound();
            }
            
            var bookingDto = new BookingDto {
                id = booking.Id,
                userId = booking.UserId,
                serviceId = booking.ServiceId,
                bookingDate = booking.BookingDate.ToString("o"),
                status = booking.Status.ToString(),
                service = booking.Service != null ? new ServiceDto {
                    trainerId = booking.Service.TrainerId,
                    serviceName = booking.Service.ServiceName ?? string.Empty,
                    start = booking.Service.Start.ToString("o"),
                    end = booking.Service.End.ToString("o"),
                    roomId = booking.Service.RoomId,
                    room = booking.Service.Room != null ? new RoomDto {
                        name = booking.Service.Room.Name,
                        capacity = booking.Service.Room.Capacity
                    } : null,
                    users = booking.Service.Users ?? new List<int>()
                } : null
            };
            
            return Ok(bookingDto);
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingDto booking)
        {
            _logger.LogInformation("Received booking: {@Booking}", booking);
            if (booking == null)
            {
                return BadRequest("Booking data is missing.");
            }

            // First create the booking
            var createdBooking = await _bookingService.CreateBooking(booking);
            
            // Then retrieve it with full relations
            var fullBooking = await _bookingService.GetBookingById(createdBooking.Id);
            
            // Map to DTO for response
            var responseDto = new BookingDto {
                id = fullBooking.Id,
                userId = fullBooking.UserId,
                serviceId = fullBooking.ServiceId,
                bookingDate = fullBooking.BookingDate.ToString("o"),
                status = fullBooking.Status.ToString(),
                service = fullBooking.Service != null ? new ServiceDto {
                    trainerId = fullBooking.Service.TrainerId,
                    serviceName = fullBooking.Service.ServiceName ?? string.Empty,
                    start = fullBooking.Service.Start.ToString("o"),
                    end = fullBooking.Service.End.ToString("o"),
                    roomId = fullBooking.Service.RoomId,
                    room = fullBooking.Service.Room != null ? new RoomDto {
                        name = fullBooking.Service.Room.Name,
                        capacity = fullBooking.Service.Room.Capacity
                    } : null,
                    users = fullBooking.Service.Users ?? new List<int>()
                } : null
            };
            
            // CreatedAtAction returns a 201 response with a Location header.
            return CreatedAtAction(nameof(GetBookingById), new { id = fullBooking.Id }, responseDto);
        }

        // DELETE: api/Bookings/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<BookingDto>> DeleteBooking(int id)
        {
            var deletedBooking = await _bookingService.DeleteBooking(id);
            if (deletedBooking == null)
            {
                return NotFound();
            }
            
            var bookingDto = new BookingDto {
                id = deletedBooking.Id,
                userId = deletedBooking.UserId,
                serviceId = deletedBooking.ServiceId,
                bookingDate = deletedBooking.BookingDate.ToString("o"),
                status = deletedBooking.Status.ToString(),
                service = deletedBooking.Service != null ? new ServiceDto {
                    trainerId = deletedBooking.Service.TrainerId,
                    serviceName = deletedBooking.Service.ServiceName ?? string.Empty,
                    start = deletedBooking.Service.Start.ToString("o"),
                    end = deletedBooking.Service.End.ToString("o"),
                    roomId = deletedBooking.Service.RoomId,
                    room = deletedBooking.Service.Room != null ? new RoomDto {
                        name = deletedBooking.Service.Room.Name,
                        capacity = deletedBooking.Service.Room.Capacity
                    } : null,
                    users = deletedBooking.Service.Users ?? new List<int>()
                } : null
            };
            
            return Ok(bookingDto);
        }

        [HttpGet("triggerSync")]
        public async Task<IActionResult> TriggerSync()
        {
            _logger.LogInformation("Sync triggered.");
            await _databaseSyncService.SyncDatabase();
            return Ok("Sync triggered successfully.");
        }
    }
}