using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Interface
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetAll();
        Task<Room?> GetById(int id);
        Task Create(Room room);
        Task Update(Room room);
        Task<Room?> Delete(int id);
        Task SaveChanges();
    }
} 
