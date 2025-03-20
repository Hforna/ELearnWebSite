using Payment.Application.Requests;
using Payment.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.ApplicationServices.Interfaces
{
    public interface IOrderService
    {
        public Task<OrderItemResponse> AddCourseToOrder(AddCourseToOrderRequest request);
        public Task<OrderResponse> GetUserOrder();
    }
}
