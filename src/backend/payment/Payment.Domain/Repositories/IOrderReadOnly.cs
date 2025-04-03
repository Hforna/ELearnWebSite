using Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Payment.Domain.Repositories
{
    public interface IOrderReadOnly
    {
        public Task<bool> OrderItemExists(long courseId, long userId);
        public Task<Order?> OrderByUserId(long userId);
        public IPagedList<Order> GetOrdersNotActive(int page, int quantity, long userId);
        public Task<List<Order>?> GetOrdersByUserId(long userId);
    }
}
