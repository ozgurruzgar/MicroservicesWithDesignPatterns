using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Dtos;
using Order.API.Models;
using Shared.Events;
using Shared.Messages;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(AppDbContext appDbContext, IPublishEndpoint publishEndpoint)
        {
            _appDbContext = appDbContext;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto orderCreateDto)
        {
            var newOrder = new Models.Order
            {
                BuyerId = orderCreateDto.BuyerId,
                Status = OrderStatus.Suspend,
                Address = new Address { District = orderCreateDto.Address.District, Province = orderCreateDto.Address.Province, Line = orderCreateDto.Address.Line },
                CreatedDate = DateTime.Now,
            };

            orderCreateDto.orderItems.ForEach(item =>
            {
                newOrder.Items.Add(new OrderItem { Count = item.Count, ProductId = item.ProductId, Price = item.Price });
            });

            await _appDbContext.AddAsync(newOrder);
            await _appDbContext.SaveChangesAsync();

            OrderCreatedEvent orderCreatedEvent = new()
            {
                BuyerId = newOrder.BuyerId,
                OrderId = newOrder.Id,
                PaymentMessage = new PaymentMessage
                {
                    CardName = orderCreateDto.payment.CardName,
                    CardNumber = orderCreateDto.payment.CardNumber,
                    Expiration = orderCreateDto.payment.Expiration,
                    TotalPrice = orderCreateDto.orderItems.Sum(x => x.Price * x.Count),
                    CVV = orderCreateDto.payment.CVV,
                },
            };

            foreach (var item in orderCreateDto.orderItems)
            {
                orderCreatedEvent.OrderItems.Add(new OrderItemMessage { Count = item.Count, ProductId = item.ProductId });
            }

            await _publishEndpoint.Publish(orderCreatedEvent);
            return Ok();
        }
    }
}
