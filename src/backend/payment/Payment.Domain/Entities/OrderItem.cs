using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public long CourseId { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; } = true;
        public Order Order { get; set; }
    }
}
