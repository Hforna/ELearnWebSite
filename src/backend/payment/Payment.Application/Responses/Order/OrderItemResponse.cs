using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Responses.Order
{
    public class OrderItemResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public string CourseId { get; set; }
        public decimal Price { get; set; }
        public string CurrencyType { get; set; }
    }
}
