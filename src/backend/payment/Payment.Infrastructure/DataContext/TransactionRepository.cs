using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
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

        public async Task<Transaction?> TransactionByGatewayId(string gatewayId)
        {
            return await _dbContext.Transactions.Include(d => d.Order).ThenInclude(d => d.OrderItems).SingleOrDefaultAsync(d => d.GatewayTransactionId == gatewayId);
        }

        public async Task<Transaction?> TransactionByOrderId(Guid orderId)
        {
            return await _dbContext.Transactions.SingleOrDefaultAsync(d => d.OrderId == orderId);
        }

        public async Task<Transaction?> TransactionByUserId(long userId)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(d => d.UserId == userId && d.Active);

            return await _dbContext.Transactions.FirstOrDefaultAsync(d => d.OrderId == order.Id && d.TransactionStatus == Domain.Enums.TransactionStatusEnum.Pending);
        }

        public async Task<List<Transaction>> TransactionsByOrderIds(List<Guid> orderIds)
        {
            return await _dbContext.Transactions.Where(d => orderIds.Contains(d.Id)).ToListAsync();
        }

        public async Task<List<Transaction>?> TransactionsByUserId(long userId)
        {
            return await _dbContext.Transactions.Where(d => d.Order.UserId == userId).ToListAsync();
        }

        public async Task<TransactionStatusEnum> TransactionStatusByOrderId(Guid orderId)
        {
            return await _dbContext.Transactions.AsNoTracking().SingleOrDefaultAsync(d => d.OrderId == orderId).Select(d => d.TransactionStatus);
        }

        public void Update(Transaction transaction)
        {
            _dbContext.Transactions.Update(transaction);
        }
    }
}
