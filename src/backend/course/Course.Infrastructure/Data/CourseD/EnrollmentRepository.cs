﻿using Course.Domain.Repositories;
using Course.Infrastructure.Data.Course;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data.CourseD
{
    public class EnrollmentRepository : IEnrollmentReadOnly, IEnrollmentWriteOnly
    {
        private readonly CourseDbContext _dbContext;

        public EnrollmentRepository(CourseDbContext dbContext) => _dbContext = dbContext;

        public async Task<bool> UserGotCourse(long courseId, long userId)
        {
            return await _dbContext.Enrollments.Where(d => d.CourseId == courseId && d.CustomerId == userId && d.Active)
                .SingleOrDefaultAsync() != null ? true : false;
        }
    }
}
