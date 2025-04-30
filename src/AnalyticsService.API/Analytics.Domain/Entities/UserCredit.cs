namespace Analytics.Domain.Entities
{
    public class UserCredit
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public long CreditBalance { get; set; }
    }
}