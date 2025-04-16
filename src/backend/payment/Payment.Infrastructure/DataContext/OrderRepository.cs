using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Payment.Infrastructure.DataContext
{
    public class OrderRepository : IOrderReadOnly, IOrderWriteOnly
    {
        private readonly PaymentDbContext _dbContext;

        public OrderRepository(PaymentDbContext dbContext) => _dbContext = dbContext;

        public async Task AddOrder(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
        }

        public async Task AddOrderItem(OrderItem orderItem)
        {
            await _dbContext.OrderItems.AddAsync(orderItem);
        }

        public void DeleteOrderRange(List<Order> orders)
        {
            _dbContext.Orders.RemoveRange(orders);
        }

        public async Task<List<Order>?> GetOrdersByUserId(long userId)
        {
            return await _dbContext.Orders.Include(d => d.OrderItems).Where(d => d.UserId == userId).ToListAsync();
        }

        public IPagedList<Order> GetOrdersNotActive(int page, int quantity, long userId)
        {
            return _dbContext.Orders.AsNoTracking().Where(d => d.Active == false && d.UserId == userId).ToPagedList(page, quantity);
        }

        public async Task<OrderItem?> LastCourseOrderItem(long courseId)
        {
            return await _dbContext.OrderItems.OrderBy(d => d.CreatedAt).LastOrDefaultAsync(d => d.CourseId == courseId && d.);
        }

        public async Task<Order?> OrderByUserId(long userId)
        {
            return await _dbContext.Orders.Include(d => d.OrderItems).SingleOrDefaultAsync(d => d.UserId == userId && d.Active);
        }

        public async Task<bool> OrderItemExists(long courseId, long userId)
        {
            return await _dbContext.OrderItems
                .Include(d => d.Order)
                .AnyAsync(d => d.Order.UserId == userId && d.CourseId == courseId);
        }

        public void UpdateOrder(Order order)
        {
            _dbContext.Orders.Update(order);
        }

        public void UpdateOrderItem(OrderItem orderItem)
        {
            _dbContext.OrderItems.Update(orderItem);
        }

        public void UpdateOrderItemRange(List<OrderItem> orderItems)
        {
            _dbContext.OrderItems.UpdateRange(orderItems);
        }
    }
}
