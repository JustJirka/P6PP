using System.Threading.Tasks;
using Dapper;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;

namespace Analytics.Infrastructure.Data.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DapperContext _context;

        public BookingRepository(DapperContext context)
        {
            _context = context;
        }

        // Gets all bookings.
        public async Task<List<Booking>> GetAll()
        {
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
                 SELECT
                    Id,
                    BookingDate,
                    Status,
                    UserId,
                    ServiceId
                 FROM Bookings;";

            var bookings = await connection.QueryAsync<Booking>(query);
            return bookings.ToList();
        }


        // Gets a single booking by id.
        public async Task<Booking?> GetById(int id)
        {
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
                SELECT
                    Id,
                    BookingDate,
                    Status,
                    UserId,
                    ServiceId
                FROM Bookings WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Booking>(query, new { Id = id });
        }

        // Creates a new booking record.
        public async Task Create(Booking booking)
        {
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
                INSERT INTO Bookings (BookingDate, Status, UserId, ServiceId)
                VALUES (@BookingDate, @Status, @UserId, @ServiceId);";

            await connection.ExecuteAsync(query, new
            {
                BookingDate = booking.BookingDate,
                Status = (int)booking.Status,
                UserId = booking.UserId,
                ServiceId = booking.ServiceId,
            });
        }

        // Deletes a user by id and returns the deleted user.
        public async Task<Booking?> Delete(int id)
        {
            using var connection = await _context.CreateConnectionAsync();
            const string selectQuery = @"SELECT * FROM Bookings WHERE Id = @Id";

            // Retrieve the booking so that we can return it after deletion.
            var booking = await connection.QuerySingleOrDefaultAsync<Booking>(selectQuery, new { Id = id });
            if (booking == null)
            {
                return null;
            }

            const string deleteQuery = @"DELETE FROM Bookings WHERE Id = @Id";
            await connection.ExecuteAsync(deleteQuery, new { Id = id });
            return booking;
        }

        // In Dapper, commands are executed immediately so SaveChanges is a no-op.
        public Task SaveChanges()
        {
            return Task.CompletedTask;
        }
    }
}
