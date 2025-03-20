using Bogus;
using CommonUtilities.Builds;
using Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Fakers.Entities
{
    public static class OrderEntityFaker
    {
        public static Order Build(long userId)
        {
            var orderId = Guid.NewGuid();
            return new Faker<Order>()
                .RuleFor(d => d.Active, true)
                .RuleFor(d => d.TotalPrice, f => f.Finance.Amount(20, 2000, 2))
                .RuleFor(d => d.Id, orderId)
                .RuleFor(d => d.UserId, userId)
                .RuleFor(d => d.CreatedAt, DateTime.UtcNow)
                .RuleFor(d => d.OrderItems, OrderItemEntityFaker.CreateRangeOrderItem(orderId));
        }
    }
}
