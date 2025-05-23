﻿using Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Repositories
{
    public interface IOrderWriteOnly
    {
        public Task AddOrderItem(OrderItem orderItem);
        public Task AddOrder(Order order);
        public void UpdateOrder(Order order);
        public void UpdateOrderItem(OrderItem orderItem);
        public void UpdateOrderItemRange(List<OrderItem> orderItems);
        public void DeleteOrderRange(List<Order> orders);
    }
}
