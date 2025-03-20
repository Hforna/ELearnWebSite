using Moq;
using Payment.Domain.Entities;
using Payment.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Builds.Repositories.OrderMock
{
    public class OrderReadOnlyMock
    {
        private readonly Mock<IOrderReadOnly> _mock = new Mock<IOrderReadOnly>();

        public IOrderReadOnly Build() => _mock.Object;

        public void OrderItemExists(long courseId, long userId, bool exists = false)
        {
            _mock.Setup(d => d.OrderItemExists(courseId, userId)).ReturnsAsync(exists);
        }

        public void OrderByUser(Order order)
        {
            _mock.Setup(d => d.OrderByUserId(order.UserId)).ReturnsAsync(order);
        }
    }
}
