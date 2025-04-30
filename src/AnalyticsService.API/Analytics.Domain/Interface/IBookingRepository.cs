using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Interface
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAll();
        Task<Booking?> GetById(int id);
        Task Create(Booking booking);
        Task<Booking?> Delete(int id);
        Task SaveChanges();
    }
}