using MassTransit;
using Shared.Messages;

namespace Shared.Interfaces
{
    public interface IOrderCreatedEvent: CorrelatedBy<Guid>
    {
        List<OrderItemMessage> OrderItems { get; set; }
    }
}
