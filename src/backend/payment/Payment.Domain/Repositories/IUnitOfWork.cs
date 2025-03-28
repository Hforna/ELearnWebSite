using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public IOrderReadOnly orderRead { get; set; }
        public IOrderWriteOnly orderWrite { get; set; }
        public ITransactionReadOnly transactionRead { get; set; }
        public ITransactionWriteOnly transactionWrite { get; set; }
        public IPaymentReadOnly paymentRead { get; set; }
        public IPaymentWriteOnly paymentWrite { get; set; }
        public IBalanceReadOnly balanceRead { get; set; }
        public IBalanceWriteOnly balanceWrite { get; set; }

        public Task Commit();
    }
}
