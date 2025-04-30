namespace Analytics.Application.DTOs
{
    public class PaymentDto
    {
        public int id { get; set; }
        public long userId { get; set; }
        public long roleId { get; set; }
        public long price { get; set; }
        public long creditAmount { get; set; }
        public string status { get; set; } = string.Empty;
        public string transactionType { get; set; } = string.Empty;
        public string createdAt { get; set; } = string.Empty;
    }
}