using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;

namespace Analytics.Infrastructure.Data.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly DapperContext _context;

        public RoomRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAll()
        {
            using var connection = await _context.CreateConnectionAsync();
            
            const string query = @"SELECT Id, Name, Capacity, Status FROM Rooms";
            
            var rooms = await connection.QueryAsync<Room>(query);
            return rooms.AsList();
        }

        public async Task<Room?> GetById(int id)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            const string query = @"SELECT Id, Name, Capacity, Status FROM Rooms WHERE Id = @Id";
            
            var room = await connection.QuerySingleOrDefaultAsync<Room>(query, new { Id = id });
            return room;
        }

        public async Task Create(Room room)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            const string checkQuery = "SELECT COUNT(1) FROM Rooms WHERE Id = @Id";
            var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { Id = room.Id }) > 0;
            
            string query;
            if (exists)
            {
                query = @"
                    UPDATE Rooms 
                    SET Name = @Name, Capacity = @Capacity, Status = @Status
                    WHERE Id = @Id;";
            }
            else
            {
                query = @"
                    INSERT INTO Rooms (Id, Name, Capacity, Status)
                    VALUES (@Id, @Name, @Capacity, @Status);";
            }

            await connection.ExecuteAsync(query, new
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Status = room.Status
            });
        }

        public async Task Update(Room room)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            const string query = @"
                UPDATE Rooms 
                SET Name = @Name, Capacity = @Capacity, Status = @Status
                WHERE Id = @Id;";
                
            await connection.ExecuteAsync(query, new
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Status = room.Status
            });
        }

        public async Task<Room?> Delete(int id)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            var room = await GetById(id);
            if (room == null)
            {
                return null;
            }

            const string deleteQuery = @"DELETE FROM Rooms WHERE Id = @Id";
            await connection.ExecuteAsync(deleteQuery, new { Id = id });
            
            return room;
        }

        public Task SaveChanges()
        {
            return Task.CompletedTask;
        }
    }
} 
