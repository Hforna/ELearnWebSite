using Bogus;
using Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Fakers.Entities
{
    public static class OrderItemEntityFaker
    {
        public static OrderItem Build(Guid orderId)
        {
            return new Faker<OrderItem>()
                .RuleFor(d => d.CourseId, f => f.Random.Long(2, 20000))
                .RuleFor(d => d.Id, Guid.NewGuid())
                .RuleFor(d => d.Active, true)
                .RuleFor(d => d.OrderId, orderId)
                .RuleFor(d => d.CreatedAt, DateTime.UtcNow)
                .RuleFor(d => d.Price, f => f.Finance.Amount(2, 2000, 2));
        }

        public static List<OrderItem> CreateRangeOrderItem(Guid orderId)
        {
            var list = new List<OrderItem>();

            for(var i = 0; i < 10; i++)
            {
                list.Add(new Faker<OrderItem>()
                .RuleFor(d => d.CourseId, f => f.Random.Long(2, 200000))
                .RuleFor(d => d.Id, Guid.NewGuid())
                .RuleFor(d => d.Active, true)
                .RuleFor(d => d.OrderId, orderId)
                .RuleFor(d => d.CreatedAt, DateTime.UtcNow)
                .RuleFor(d => d.Price, f => f.Finance.Amount(2, 2000, 2)));
            }
            return list;
        }
    }
}
