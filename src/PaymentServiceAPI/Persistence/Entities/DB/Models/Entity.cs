using PaymentService.API.Persistence.Entities.DB.Interfaces;

namespace PaymentService.API.Persistence.Entities.DB.Models;

public class Entity : IEntity<int>
{
    public int Id { get; set; }
}
