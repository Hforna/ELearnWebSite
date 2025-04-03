using Payment.Domain.Entities;
using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Repositories
{
    public interface ITransactionReadOnly
    {
        public Task<bool> ThereIsOrderTransactionActive(Guid orderId);
        public Task<Transaction?> TransactionByUserId(long userId);
        public Task<Transaction?> TransactionByGatewayId(string gatewayId);
        public Task<TransactionStatusEnum> TransactionStatusByOrderId(Guid orderId);
        public Task<List<Transaction>> TransactionsByOrderIds(List<Guid> orderIds);
        public Task<Transaction?> TransactionByOrderId(Guid orderId);
        public Task<List<Transaction>?> TransactionsByUserId(long userId);
    }
}
