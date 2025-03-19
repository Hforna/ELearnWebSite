using Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Repositories
{
    public interface IOrderReadOnly
    {
        public Task<bool> OrderItemExists(long courseId, long userId);
    }
}
