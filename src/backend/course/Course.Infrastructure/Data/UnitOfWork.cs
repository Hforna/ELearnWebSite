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
        public ICourseReadOnly courseRead { get; set; }
        public ICourseWriteOnly courseWrite { get; set; }

        public UnitOfWork(CourseDbContext dbContext, ICourseReadOnly courseRead, ICourseWriteOnly courseWrite)
        {
            _dbContext = dbContext;
            this.courseRead = courseRead;
            this.courseWrite = courseWrite;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
