namespace PaymentService.API.Persistence.Entities.DB.Interfaces
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
