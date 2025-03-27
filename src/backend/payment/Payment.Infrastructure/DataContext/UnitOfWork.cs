﻿using Payment.Domain.Repositories;
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

        public UnitOfWork(PaymentDbContext dbContext, IOrderReadOnly orderRead, IOrderWriteOnly orderWrite,
            ITransactionReadOnly transactionRead, ITransactionWriteOnly transactionWrite, 
            IPaymentReadOnly paymentRead, IPaymentWriteOnly paymentWrite)
        {
            _dbContext = dbContext;
            this.orderWrite = orderWrite;
            this.orderRead = orderRead;
            this.transactionRead = transactionRead;
            this.transactionWrite = transactionWrite;
            this.paymentWrite = paymentWrite;
            this.paymentRead = paymentRead;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
