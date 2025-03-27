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
    public class TransactionRepository : ITransactionWriteOnly, ITransactionReadOnly
    {
        private readonly PaymentDbContext _dbContext;

        public TransactionRepository(PaymentDbContext dbContext) => _dbContext = dbContext;

        public async Task Add(Transaction transaction)
        {
            await _dbContext.Transactions.AddAsync(transaction);
        }

        public async Task<bool> ThereIsOrderTransactionActive(Guid orderId)
        {
            return await _dbContext.Transactions
                .AnyAsync(d => d.OrderId == orderId && d.TransactionStatus == Domain.Enums.TransactionStatusEnum.Pending);
        }
    }
}
