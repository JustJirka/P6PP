using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Interface
{
    public interface IServiceRepository
    {
        Task<List<Service>> GetAll();
        Task<Service?> GetById(int id);
        Task Create(Service service);
        Task Update(Service service);
        Task<Service?> Delete(int id);
        Task UpdateUsers(int serviceId, List<int> userIds);
        Task SaveChanges();
    }
} 
