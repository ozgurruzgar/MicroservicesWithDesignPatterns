using MassTransit;
using Shared;
using Shared.Events;
using Shared.Interfaces;
using Shared.Messages;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }
        public Event<IStockNotReservedEvent> StockNotReservedEvent { get; set; }
        public Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public Event<IPaymentFailedEvent> PaymentFailedEvent { get; set; }

        public State OrderCreated { get; set; }
        public State StockReserved { get; set; }
        public State StockNotReserved { get; set; }
        public State PaymentCompleted { get; set; }
        public State PaymentFailed { get; set; }

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderCreatedRequestEvent, y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId).SelectId(context => Guid.NewGuid()));

            Event(() => StockReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
            
            Event(() => StockNotReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

            Event(() => PaymentCompletedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

            Initially(When(OrderCreatedRequestEvent).Then(context =>
            {
                context.Saga.BuyerId = context.Message.BuyerId;
                context.Saga.OrderId = context.Message.OrderId;
                context.Saga.CreatedDate = DateTime.Now;
                context.Saga.CardName = context.Message.Payment.CardName;
                context.Saga.CardNumber = context.Message.Payment.CardNumber;
                context.Saga.CVV = context.Message.Payment.CVV;
                context.Saga.Expiration = context.Message.Payment.Expiration;
                context.Saga.TotalPrice = context.Message.Payment.TotalPrice;
            })
            .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent before : {context.Saga}"); })
            .Publish(context => new OrderCreatedEvent(context.Saga.CorrelationId) { OrderItems = context.Message.OrderItems})
            .TransitionTo(OrderCreated)
            .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent after" +
                $" : {context.Saga}"); }));


            During(OrderCreated, 
                When(StockReservedEvent)
                .TransitionTo(StockReserved)
                .Send(new Uri($"queue:{RabbitMQSettings.PaymentStockReservedRequestQueueName}"),context => new StockReservedRequestPayment(context.Saga.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems,
                    Payment = new Shared.Messages.PaymentMessage 
                    {
                        CardName = context.Saga.CardName,
                        CardNumber = context.Saga.CardNumber,
                        Expiration = context.Saga.Expiration,
                        CVV = context.Saga.CVV,
                        TotalPrice = context.Saga.TotalPrice,
                    },
                    BuyerId = context.Saga.BuyerId,
                })
                .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent after" + $" : {context.Saga}"); }),
                When(StockNotReservedEvent)
                .TransitionTo(StockNotReserved)
                .Publish(context => new OrderRequestFailedEvent { Reason = context.Message.Reason, OrderId = context.Saga.OrderId})
                .Then(context => { Console.WriteLine($"OrderRequestFailedEvent after" + $" : {context.Saga}"); }));


            During(StockReserved, When(PaymentCompletedEvent)
                .TransitionTo(PaymentCompleted)
                .Publish(context => new OrderRequestCompletedEvent() { OrderId = context.Saga.OrderId })
                .Then(context => { Console.WriteLine($"PaymentCompletedEvent after" + $" : {context.Saga}"); }).Finalize(),
                When(PaymentFailedEvent)
                .Publish(context => new OrderRequestFailedEvent { Reason = context.Message.Reason, OrderId = context.Saga.OrderId })
                .Send(new Uri($"queue:{RabbitMQSettings.StockRollBackMessageQueueName}"), context => new StockRollbackMessage { OrderItems = context.Message.OrderItems})
                .TransitionTo(PaymentFailed)
                .Then(context => { Console.WriteLine($"PaymentFailedEvent after" + $" : {context.Saga}"); }));

            SetCompletedWhenFinalized();
        }
    }
}
