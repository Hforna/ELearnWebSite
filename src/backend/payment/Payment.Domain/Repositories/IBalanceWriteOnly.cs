using Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Repositories
{
    public interface IBalanceWriteOnly
    {
        public Task Add(Balance balance);
        public void Update(Balance balance);
        public void UpdateRange(ICollection<Balance> balances);
        public Task AddBlockedBalance(BlockedBalance blockedBalance);
        public void DeletePayoutRange(List<Payout> payouts);
        public void DeleteBlockedBalance(BlockedBalance blockedBalance);
        public void Delete(Balance balance);
    }
}
