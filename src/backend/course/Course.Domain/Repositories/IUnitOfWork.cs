using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public ICourseReadOnly courseRead { get; set; }
        public ICourseWriteOnly courseWrite { get; set; }
        public Task Commit();
    }
}
