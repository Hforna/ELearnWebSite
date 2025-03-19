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
        public Task Commit();
    }
}
