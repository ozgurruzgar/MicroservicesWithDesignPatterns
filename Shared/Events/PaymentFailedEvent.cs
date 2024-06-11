using Shared.Interfaces;
using Shared.Messages;

namespace Shared.Events
{
    public class PaymentFailedEvent : IPaymentFailedEvent
    {
        public PaymentFailedEvent(Guid correaltionId)
        {
            CorrelationId = correaltionId;
        }

        public string Reason { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; }
    }
}
