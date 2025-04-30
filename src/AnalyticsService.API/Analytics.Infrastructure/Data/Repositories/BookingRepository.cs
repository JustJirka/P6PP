using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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
            
            // SQL query that joins Bookings with Services and Rooms
            const string query = @"
                SELECT 
                    b.Id, b.BookingDate, b.Status, b.UserId, b.ServiceId,
                    s.Id as ServiceId, s.Start, s.End, s.Price, s.ServiceName, s.IsCancelled, s.TrainerId, s.RoomId,
                    r.Id as RoomId, r.Name, r.Capacity, r.Status as RoomStatus
                FROM Bookings b
                LEFT JOIN Services s ON b.ServiceId = s.Id
                LEFT JOIN Rooms r ON s.RoomId = r.Id;";

            // Dictionary to track unique bookings
            var bookingDict = new Dictionary<int, Booking>();
            
            await connection.QueryAsync<Booking, Service, Room, Booking>(
                query,
                (booking, service, room) => 
                {
                    // Check if we've already created this booking
                    if (!bookingDict.TryGetValue(booking.Id, out var bookingEntry))
                    {
                        bookingEntry = booking;
                        bookingEntry.Service = service;
                        bookingDict.Add(booking.Id, bookingEntry);
                    }
                    
                    // Add room to service if it exists
                    if (bookingEntry.Service != null && room != null)
                    {
                        bookingEntry.Service.Room = room;
                    }
                    
                    return bookingEntry;
                },
                splitOn: "ServiceId,RoomId"); // Important! Split on the right columns
                
            // Also fetch Users for each Service
            foreach (var booking in bookingDict.Values)
            {
                if (booking.Service != null)
                {
                    const string usersQuery = @"
                        SELECT UserId FROM ServiceUsers WHERE ServiceId = @ServiceId";
                        
                    var users = await connection.QueryAsync<int>(usersQuery, new { ServiceId = booking.ServiceId });
                    booking.Service.Users = users.ToList();
                }
            }

            return bookingDict.Values.ToList();
        }

        // Gets a single booking by id.
        public async Task<Booking?> GetById(int id)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            // Updated query to include joined data
            const string query = @"
                SELECT 
                    b.Id, b.BookingDate, b.Status, b.UserId, b.ServiceId,
                    s.Id as ServiceId, s.Start, s.End, s.Price, s.ServiceName, s.IsCancelled, s.TrainerId, s.RoomId,
                    r.Id as RoomId, r.Name, r.Capacity, r.Status as RoomStatus
                FROM Bookings b
                LEFT JOIN Services s ON b.ServiceId = s.Id
                LEFT JOIN Rooms r ON s.RoomId = r.Id
                WHERE b.Id = @Id;";

            Booking? result = null;
            
            await connection.QueryAsync<Booking, Service, Room, Booking>(
                query,
                (booking, service, room) => 
                {
                    if (result == null)
                    {
                        result = booking;
                        result.Service = service;
                    }
                    
                    // Add room to service if it exists
                    if (result.Service != null && room != null)
                    {
                        result.Service.Room = room;
                    }
                    
                    return result;
                },
                new { Id = id },
                splitOn: "ServiceId,RoomId");
                
            // Also fetch Users for the Service
            if (result?.Service != null)
            {
                const string usersQuery = @"
                    SELECT UserId FROM ServiceUsers WHERE ServiceId = @ServiceId";
                    
                var users = await connection.QueryAsync<int>(usersQuery, new { ServiceId = result.ServiceId });
                result.Service.Users = users.ToList();
            }

            return result;
        }

        // Creates a new booking record.
        public async Task Create(Booking booking)
        {
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
                INSERT INTO Bookings (BookingDate, Status, UserId, ServiceId)
                VALUES (@BookingDate, @Status, @UserId, @ServiceId);
                SELECT LAST_INSERT_ID();";

            var id = await connection.ExecuteScalarAsync<int>(query, new
            {
                BookingDate = booking.BookingDate,
                Status = (int)booking.Status,
                UserId = booking.UserId,
                ServiceId = booking.ServiceId,
            });
            
            booking.Id = id;
        }

        // Deletes a user by id and returns the deleted user.
        public async Task<Booking?> Delete(int id)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            // Get full booking with joined data before deleting
            var booking = await GetById(id);
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