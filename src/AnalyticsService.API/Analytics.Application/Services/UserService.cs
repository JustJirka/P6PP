using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Application.Services.Interface;
using Analytics.Domain.Entities;
using Analytics.Domain.Interface;
using Analytics.Application.DTOs;
using Analytics.Domain.Enums;

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
            //if (string.IsNullOrEmpty(user.birthDate))
            //    throw new ArgumentNullException(nameof(user.birthDate), "Birth date cannot be null or empty.");
            if (string.IsNullOrEmpty(user.createdAt))
                throw new ArgumentNullException(nameof(user.createdAt), "Created at date cannot be null or empty.");
            if (string.IsNullOrEmpty(user.lastUpdated))
                throw new ArgumentNullException(nameof(user.lastUpdated), "Last updated date cannot be null or empty.");

            var newUser = new User()
            {
                Id = user.id,
                RoleId = user.roleId,
                State = user.state,
                Sex = string.IsNullOrEmpty(user.sex) ? (Sex?)null : (user.sex == "male" ? Sex.male : Sex.female),
                Weight = user.weight ?? 0,
                Height = user.height ?? 0,
                BirthDate = !string.IsNullOrEmpty(user.birthDate) ? DateTime.Parse(user.birthDate) : default(DateTime),
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
