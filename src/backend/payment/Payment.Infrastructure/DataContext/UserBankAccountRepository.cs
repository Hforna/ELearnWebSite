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
    public class UserBankAccountRepository : IUserBankAccountReadOnly, IUserBankAccountWriteOnly
    {
        private readonly PaymentDbContext _dbContext;

        public UserBankAccountRepository(PaymentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(UserBankAccount bankAccount)
        {
            await _dbContext.UserBankAccounts.AddAsync(bankAccount);    
        }

        public async Task<UserBankAccount?> UserBankAccountByIdAndUserId(Guid bankId, long userId)
        {
            return await _dbContext.UserBankAccounts.SingleOrDefaultAsync(d => d.TeacherId == userId && d.Id == bankId);
        }
    }
}
