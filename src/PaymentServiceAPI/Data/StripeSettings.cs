namespace PaymentService.API.Data
{
    public class StripeSettings
    {

        public string SecretKey { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty; 
    }
}
