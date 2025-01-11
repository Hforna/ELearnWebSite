using Course.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CourseDbContext _dbContext;
        private readonly ICourseReadOnly _courseRead;
        private readonly ICourseWriteOnly _courseWrite;

        public UnitOfWork(CourseDbContext dbContext, ICourseReadOnly courseRead, ICourseWriteOnly courseWrite)
        {
            _dbContext = dbContext;
            _courseRead = courseRead;
            _courseWrite = courseWrite;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
