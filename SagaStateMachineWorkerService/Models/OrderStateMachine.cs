﻿using Automatonymous;
using Shared.Events;
using Shared.Interfaces;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }

        public State OrderCreated { get; set; }
        public State StockReserved { get; set; }

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderCreatedRequestEvent, y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId).SelectId(context => Guid.NewGuid()));

            Initially(When(OrderCreatedRequestEvent).Then(context =>
            {
                context.Instance.BuyerId = context.Data.BuyerId;
                context.Instance.OrderId = context.Data.OrderId;
                context.Instance.CreatedDate = DateTime.Now;
                context.Instance.CardName = context.Data.Payment.CardName;
                context.Instance.CardNumber = context.Data.Payment.CardNumber;
                context.Instance.CVV = context.Data.Payment.CVV;
                context.Instance.Expiration = context.Data.Payment.Expiration;
                context.Instance.TotalPrice = context.Data.Payment.TotalPrice;
            })
            .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent before : {context.Instance}"); })
            .Publish(context => new OrderCreatedEvent(context.Instance.CorrelationId) { OrderItems = context.Data.OrderItems})
            .TransitionTo(OrderCreated)
            .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent after" +
                $" : {context.Instance}"); }));
        }
    }
}