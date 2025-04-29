using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;

namespace Analytics.Infrastructure.Data.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly DapperContext _context;

        public ServiceRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<List<Service>> GetAll()
        {
            using var connection = await _context.CreateConnectionAsync();
            
            const string query = @"
                SELECT 
                    s.Id, s.ServiceName, s.Start, s.End, s.Price, s.IsCancelled, s.TrainerId, s.RoomId,
                    r.Id as RoomId, r.Name, r.Capacity, r.Status as RoomStatus
                FROM Services s
                LEFT JOIN Rooms r ON s.RoomId = r.Id;";

            var serviceDict = new Dictionary<int, Service>();
            
            await connection.QueryAsync<Service, Room, Service>(
                query,
                (service, room) => 
                {
                    if (!serviceDict.TryGetValue(service.Id, out var serviceEntry))
                    {
                        serviceEntry = service;
                        serviceDict.Add(service.Id, serviceEntry);
                    }
                    
                    if (room != null)
                    {
                        serviceEntry.Room = room;
                    }
                    
                    return serviceEntry;
                },
                splitOn: "RoomId");
                
            foreach (var service in serviceDict.Values)
            {
                const string usersQuery = @"
                    SELECT UserId FROM ServiceUsers WHERE ServiceId = @ServiceId";
                    
                var users = await connection.QueryAsync<int>(usersQuery, new { ServiceId = service.Id });
                service.Users = users.ToList();
            }

            return serviceDict.Values.ToList();
        }

        public async Task<Service?> GetById(int id)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            const string query = @"
                SELECT 
                    s.Id, s.ServiceName, s.Start, s.End, s.Price, s.IsCancelled, s.TrainerId, s.RoomId,
                    r.Id as RoomId, r.Name, r.Capacity, r.Status as RoomStatus
                FROM Services s
                LEFT JOIN Rooms r ON s.RoomId = r.Id
                WHERE s.Id = @Id;";

            Service? result = null;
            
            await connection.QueryAsync<Service, Room, Service>(
                query,
                (service, room) => 
                {
                    if (result == null)
                    {
                        result = service;
                    }
                    
                    if (room != null)
                    {
                        result.Room = room;
                    }
                    
                    return result;
                },
                new { Id = id },
                splitOn: "RoomId");
                
            if (result != null)
            {
                const string usersQuery = @"
                    SELECT UserId FROM ServiceUsers WHERE ServiceId = @ServiceId";
                    
                var users = await connection.QueryAsync<int>(usersQuery, new { ServiceId = id });
                result.Users = users.ToList();
            }

            return result;
        }

        public async Task Create(Service service)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            const string checkQuery = "SELECT COUNT(1) FROM Services WHERE Id = @Id";
            var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { Id = service.Id }) > 0;
            
            string query;
            if (exists)
            {
                query = @"
                    UPDATE Services 
                    SET ServiceName = @ServiceName, Start = @Start, End = @End, 
                        Price = @Price, IsCancelled = @IsCancelled, 
                        TrainerId = @TrainerId, RoomId = @RoomId
                    WHERE Id = @Id;";
            }
            else
            {
                query = @"
                    INSERT INTO Services (Id, ServiceName, Start, End, Price, IsCancelled, TrainerId, RoomId)
                    VALUES (@Id, @ServiceName, @Start, @End, @Price, @IsCancelled, @TrainerId, @RoomId);";
            }

            await connection.ExecuteAsync(query, new
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Start = service.Start,
                End = service.End,
                Price = service.Price,
                IsCancelled = service.IsCancelled,
                TrainerId = service.TrainerId,
                RoomId = service.RoomId
            });
            
            if (service.Users != null && service.Users.Count > 0)
            {
                await UpdateUsers(service.Id, service.Users);
            }
        }

        public async Task Update(Service service)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            const string query = @"
                UPDATE Services 
                SET ServiceName = @ServiceName, Start = @Start, End = @End, 
                    Price = @Price, IsCancelled = @IsCancelled, 
                    TrainerId = @TrainerId, RoomId = @RoomId
                WHERE Id = @Id;";
                
            await connection.ExecuteAsync(query, new
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Start = service.Start,
                End = service.End,
                Price = service.Price,
                IsCancelled = service.IsCancelled,
                TrainerId = service.TrainerId,
                RoomId = service.RoomId
            });
            
            if (service.Users != null && service.Users.Count > 0)
            {
                await UpdateUsers(service.Id, service.Users);
            }
        }

        public async Task<Service?> Delete(int id)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            var service = await GetById(id);
            if (service == null)
            {
                return null;
            }

            const string deleteUsersQuery = @"DELETE FROM ServiceUsers WHERE ServiceId = @Id";
            await connection.ExecuteAsync(deleteUsersQuery, new { Id = id });
            
            const string deleteQuery = @"DELETE FROM Services WHERE Id = @Id";
            await connection.ExecuteAsync(deleteQuery, new { Id = id });
            
            return service;
        }
        
        public async Task UpdateUsers(int serviceId, List<int> userIds)
        {
            using var connection = await _context.CreateConnectionAsync();
            
            const string deleteQuery = @"DELETE FROM ServiceUsers WHERE ServiceId = @ServiceId";
            await connection.ExecuteAsync(deleteQuery, new { ServiceId = serviceId });
            
            if (userIds.Count > 0)
            {
                const string insertQuery = @"INSERT INTO ServiceUsers (ServiceId, UserId) VALUES (@ServiceId, @UserId)";
                
                foreach (var userId in userIds)
                {
                    await connection.ExecuteAsync(insertQuery, new { ServiceId = serviceId, UserId = userId });
                }
            }
        }

        public Task SaveChanges()
        {
            return Task.CompletedTask;
        }
    }
} 
