using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Application.DTOs;

namespace Analytics.Application.Services.Interface
{
    public interface IUserService
    {
        Task<User> GetUserById(int id);
        Task<List<User>> GetAllUsers();
        Task<User> CreateUser(UserDto user);
        Task<User> DeleteUser(int id);
    }
}
