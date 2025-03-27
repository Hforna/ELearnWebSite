using Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Repositories
{
    public interface IPaymentWriteOnly
    {
        public Task Add(PaymentEntity payment);
    }
}
