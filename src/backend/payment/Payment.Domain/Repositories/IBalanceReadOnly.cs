using Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Repositories
{
    public interface IBalanceReadOnly
    {
        public Task<bool> TeacherBalanceExists(long teacherId);
        public Task<Balance?> BalanceByTeacherId(long teacherId);
    }
}
