﻿using MassTransit;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers
{
    public class OrderRequestCompletedEventConsumer : IConsumer<OrderRequestCompletedEvent>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderRequestCompletedEventConsumer> _logger;

        public OrderRequestCompletedEventConsumer(AppDbContext context, ILogger<OrderRequestCompletedEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderRequestCompletedEvent> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);

            if (order != null)
            {
                order.Status = OrderStatus.Complete;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order (Id={context.Message.OrderId}) status changed : {order.Status}");
            }
            else
            {
                _logger.LogError($"Order (Id={context.Message.OrderId}) not found");
            }
        }
    }
}
