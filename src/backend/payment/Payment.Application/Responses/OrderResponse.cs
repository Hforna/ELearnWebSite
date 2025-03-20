using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Responses
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public List<OrderItemResponse> OrderItems { get; set; }
    }
}
