﻿using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    public class Order : BaseEntity
    {
        public long UserId { get; set; }
        public bool Active { get; set; } = true;
        public CurrencyEnum Currency { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
