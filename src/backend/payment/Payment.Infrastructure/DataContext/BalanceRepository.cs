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
    public class BalanceRepository : IBalanceReadOnly, IBalanceWriteOnly
    {
        private readonly PaymentDbContext _dbContext;

        public BalanceRepository(PaymentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> TeacherBalanceExists(long teacherId)
        {
            return await _dbContext.Balances.AnyAsync(d => d.TeacherId == teacherId);
        }

        public async Task Add(Balance balance)
        {
            await _dbContext.Balances.AddAsync(balance);
        }

        public async Task<Balance?> BalanceByTeacherId(long teacherId)
        {
            return await _dbContext.Balances.SingleOrDefaultAsync(d => d.TeacherId == teacherId);
        }

        public void Update(Balance balance)
        {
            _dbContext.Balances.Update(balance);
        }

        public void UpdateRange(ICollection<Balance> balances)
        {
            _dbContext.Balances.UpdateRange(balances);
        }

        public void Delete(Balance balance)
        {
            _dbContext.Balances.Remove(balance);
        }

        public async Task<List<Payout>?> PayoutsByUser(long userId)
        {
            return await _dbContext.Payouts.Where(d => d.UserId == userId).ToListAsync();
        }

        public void DeletePayoutRange(List<Payout> payouts)
        {
            _dbContext.Payouts.RemoveRange(payouts);
        }
    }
}
