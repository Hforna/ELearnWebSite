using Payment.Application.Requests;
using Payment.Application.Responses.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.ApplicationServices.Interfaces
{
    public interface IPaymentService
    {
        public Task<PaymentPixResponse> ProcessPixPayment(PixPaymentRequest request);
    }
}
