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

        public UnitOfWork(PaymentDbContext dbContext, IOrderReadOnly orderRead, IOrderWriteOnly orderWrite)
        {
            _dbContext = dbContext;
            this.orderRead = orderRead;
            this.orderWrite = orderWrite;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
