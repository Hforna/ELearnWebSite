using Payment.Domain.Entities;
using Payment.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.DataContext
{
    public class PayoutRepository : IPayoutReadOnly, IPayoutWriteOnly
    {
        private readonly PaymentDbContext _dbContext;

        public PayoutRepository(PaymentDbContext dbContext) => _dbContext = dbContext;

        public async Task Add(Payout payout)
        {
            await _dbContext.Payouts.AddAsync(payout);    
        }

        public async Task<List<Payout>?> PayoutRecentsByUserAndTime(long userId, DateTime timeOfPayout)
        {
            return await _dbContext.Payouts.Where(d => d.UserId == userId && d.RequestedAt.AddDays(1) >= timeOfPayout);
        }
    }
}
