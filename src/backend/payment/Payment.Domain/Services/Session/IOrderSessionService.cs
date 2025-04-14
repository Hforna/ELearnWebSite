using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Services.Session
{
    public interface IOrderSessionService
    {
        public void AddOrderToSession(long courseId);
        public Dictionary<Guid, long>? GetSessionOrder();
    }
}
