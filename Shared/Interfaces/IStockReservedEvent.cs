using MassTransit;
using Shared.Messages;

namespace Shared.Interfaces
{
    public interface IStockReservedEvent : CorrelatedBy<Guid>
    {
        List<OrderItemMessage> OrderItems { get; set; }
    }
}
