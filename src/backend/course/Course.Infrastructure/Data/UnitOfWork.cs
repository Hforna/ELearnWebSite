﻿using Course.Domain.Repositories;
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
        public ICourseReadOnly courseRead { get;  set; }
        public ICourseWriteOnly courseWrite { get;  set; }
        public IModuleWriteOnly moduleWrite { get;  set; }
        public IModuleReadOnly moduleRead { get; set; }
        public ILessonReadOnly lessonRead { get; set; }
        public ILessonWriteOnly lessonWrite { get; set; }

        public UnitOfWork(CourseDbContext dbContext, ICourseReadOnly courseRead, ICourseWriteOnly courseWrite,
            IModuleReadOnly moduleReadOnly, IModuleWriteOnly moduleWriteOnly, ILessonReadOnly lessonRead, ILessonWriteOnly lessonWrite)
        {
            _dbContext = dbContext;
            this.courseRead = courseRead;
            this.courseWrite = courseWrite;
            moduleRead = moduleReadOnly;
            moduleWrite = moduleWriteOnly;
            this.lessonRead = lessonRead;
            this.lessonWrite = lessonWrite;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
