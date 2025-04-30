namespace Analytics.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public long Price { get; set; }
        public long CreditAmount { get; set; }
        public string Status { get; set; }
        public string TransactionType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}