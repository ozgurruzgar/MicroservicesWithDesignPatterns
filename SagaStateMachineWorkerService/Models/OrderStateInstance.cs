﻿using MassTransit;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateInstance : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string BuyerId { get; set; }
        public int OrderId { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }

        public override string ToString()
        {
            var properties = GetType().GetProperties();
            var sb = new StringBuilder();
            properties.ToList().ForEach(p =>
            {
                var value = p.GetValue(this, null);
                sb.Append($"{p.Name}:{value}");
            });

            sb.AppendLine("------------------------------");
            return sb.ToString(); 
        }
    }
}
