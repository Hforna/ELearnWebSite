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
    public class PaymentRepository : IPaymentReadOnly, IPaymentWriteOnly
    {
        private readonly PaymentDbContext _dbContext;

        public PaymentRepository(PaymentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(PaymentEntity payment) => await _dbContext.Payments.AddAsync(payment);

        public void Delete(PaymentEntity payment)
        {
            _dbContext.Payments.Remove(payment);
        }

        public async Task<PaymentEntity?> PaymentByUser(long userId)
        {
            return await _dbContext.Payments.SingleOrDefaultAsync(d => d.CustomerId == userId);
        }
    }
}
