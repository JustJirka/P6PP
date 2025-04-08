using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Application.Services.Interface;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;
using Analytics.Application.DTOs;

namespace Analytics.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAll();
        }

        public async Task<User> GetUserById(int id)
        {
            // Convert Guid to string to match repository requirements.
            return await _userRepository.GetById(id);
        }

        public async Task<User> CreateUser(UserDto user)
        {
            // Ideally, the repository should return the created user (e.g., with the generated ID);
            // otherwise, return the provided user.
            var newUser = new User()
            {
                Id = user.id,
                RoleId = user.roleId,
                State = user.state,
                Sex = (Domain.Enums.Sex)user.sex,
                Weight = user.weight,
                Height = user.height,
                BirthDate = DateTime.Parse(user.birthDate),
                CreatedAt = DateTime.Parse(user.createdAt),
                LastUpdated = DateTime.Parse(user.lastUpdated)
            };
            await _userRepository.Create(newUser);
            return newUser;
        }

        public async Task<User> DeleteUser(int id)
        {
            // Convert Guid to string before calling the repository.
            return await _userRepository.Delete(id);
        }
    }
}
