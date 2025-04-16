using Analytics.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Analytics.Application.DTOs;

namespace Analytics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IDatabaseSyncService _databaseSyncService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IBookingService bookingService, IDatabaseSyncService databaseSyncService)
        {
            _bookingService = bookingService;
            _databaseSyncService = databaseSyncService;
            _logger = new LoggerFactory().CreateLogger<BookingsController>();
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookings();
            return Ok(bookings);
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
            return Ok(booking);
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateUser([FromBody] BookingDto booking)
        {
            _logger.LogInformation("Received booking: {@Booking}", booking);
            if (booking == null)
            {
                return BadRequest("Booking data is missing.");
            }

            var createdBooking = await _bookingService.CreateBooking(booking);
            // CreatedAtAction returns a 201 response with a Location header.
            return CreatedAtAction(nameof(GetBookingById), new { id = createdBooking.Id }, createdBooking);
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
            return Ok(deletedBooking);
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
