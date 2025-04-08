using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;

namespace Analytics.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        // Gets all users.
        public async Task<List<User>> GetAll()
        {
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
                 SELECT 
                    Id, 
                    RoleId, 
                    State, 
                    Sex, 
                    Weight, 
                    Height, 
                    DateOfBirth AS BirthDate, 
                    CreatedOn AS CreatedAt, 
                    UpdatedOn AS LastUpdated 
                 FROM Users;";

            var users = await connection.QueryAsync<User>(query);
            return users.ToList();
        }


        // Gets a single user by id.
        public async Task<User?> GetById(int id)
        {
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
                SELECT
                    Id, 
                    RoleId, 
                    State, 
                    Sex, 
                    Weight, 
                    Height, 
                    DateOfBirth AS BirthDate, 
                    CreatedOn AS CreatedAt, 
                    UpdatedOn AS LastUpdated
                FROM Users WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
        }

        // Creates a new user record.
        public async Task Create(User user)
        {
            using var connection = await _context.CreateConnectionAsync();
            const string query = @"
                INSERT INTO Users (RoleId, State, Sex, Weight, Height, DateOfBirth, CreatedOn, UpdatedOn)
                VALUES (@RoleId, @State, @Sex, @Weight, @Height, @DateOfBirth, @CreatedOn, @UpdatedOn);";

            // If your Sex property is an enum, you might want to store it as a string (or an int) depending on your design.
            await connection.ExecuteAsync(query, new
            {
                RoleId = user.RoleId,
                State = user.State,
                Sex = user.Sex.ToString(),   // Adjust as needed (or cast to int)
                Weight = user.Weight,
                Height = user.Height,
                DateOfBirth = user.BirthDate,  // Match property naming with table column (DateOfBirth vs. BirthDate)
                CreatedOn = user.CreatedAt,
                UpdatedOn = user.LastUpdated
            });
        }

        // Deletes a user by id and returns the deleted user.
        public async Task<User?> Delete(int id)
        {
            using var connection = await _context.CreateConnectionAsync();
            const string selectQuery = @"SELECT * FROM Users WHERE Id = @Id";

            // Retrieve the user so that we can return it after deletion.
            var user = await connection.QuerySingleOrDefaultAsync<User>(selectQuery, new { Id = id });
            if (user == null)
            {
                return null;
            }

            const string deleteQuery = @"DELETE FROM Users WHERE Id = @Id";
            await connection.ExecuteAsync(deleteQuery, new { Id = id });
            return user;
        }

        // In Dapper, commands are executed immediately so SaveChanges is a no-op.
        public Task SaveChanges()
        {
            return Task.CompletedTask;
        }
    }
}
