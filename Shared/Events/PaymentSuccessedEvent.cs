namespace Shared.Events
{
    public class PaymentSuccessedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
    }
}
