using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Interface
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
        Task<User?> GetById(int id);
        Task Create(User user);
        Task<User?> Delete(int id);
        Task SaveChanges();
    }
}
