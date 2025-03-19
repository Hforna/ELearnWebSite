using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.DataContext
{
    public class OrderRepository : IOrderReadOnly, IOrderWriteOnly
    {
        private readonly PaymentDbContext _dbContext;

        public OrderRepository(PaymentDbContext dbContext) => _dbContext = dbContext;

        public async Task AddOrderItem(OrderItem orderItem)
        {
            await _dbContext.OrderItems.AddAsync(orderItem);
        }

        public async Task<bool> OrderItemExists(long courseId, long userId)
        {
            return await _dbContext.OrderItems
                .Include(d => d.Order)
                .AnyAsync(d => d.Order.UserId == userId && d.CourseId == courseId);
        }
    }
}
