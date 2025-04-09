using Payment.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.DataContext
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentDbContext _dbContext;
        public IOrderReadOnly orderRead { get; set; }
        public IOrderWriteOnly orderWrite { get; set; }
        public ITransactionReadOnly transactionRead { get; set; }
        public ITransactionWriteOnly transactionWrite { get; set; }
        public IPaymentReadOnly paymentRead { get; set; }
        public IPaymentWriteOnly paymentWrite { get; set; }
        public IBalanceReadOnly balanceRead { get; set; }
        public IBalanceWriteOnly balanceWrite { get; set; }
        public IUserBankAccountReadOnly bankAccountRead { get; set; }
        public IUserBankAccountWriteOnly bankAccountWrite { get; set; }
        public IPayoutReadOnly payoutRead { get; set; }
        public IPayoutWriteOnly payoutWrite { get; set; }

        public UnitOfWork(PaymentDbContext dbContext, IOrderReadOnly orderRead, IOrderWriteOnly orderWrite,
            ITransactionReadOnly transactionRead, ITransactionWriteOnly transactionWrite, 
            IPaymentReadOnly paymentRead, IPaymentWriteOnly paymentWrite,
            IBalanceReadOnly balanceRead, IBalanceWriteOnly balanceWrite,
            IUserBankAccountReadOnly userBankAccountRead, IUserBankAccountWriteOnly userBankAccountWrite, IPayoutReadOnly payoutRead, IPayoutWriteOnly payoutWrite)
        {
            _dbContext = dbContext;
            bankAccountRead = userBankAccountRead;
            bankAccountWrite = userBankAccountWrite;
            this.orderWrite = orderWrite;
            this.balanceWrite = balanceWrite;
            this.balanceRead = balanceRead;
            this.orderRead = orderRead;
            this.transactionRead = transactionRead;
            this.transactionWrite = transactionWrite;
            this.paymentWrite = paymentWrite;
            this.paymentRead = paymentRead;
            this.payoutRead = payoutRead;
            this.payoutWrite = payoutWrite;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
