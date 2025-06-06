﻿using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Infrastructure.Data.Course;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Course.Infrastructure.Data.CourseD
{
    public class EnrollmentRepository : IEnrollmentReadOnly, IEnrollmentWriteOnly
    {
        private readonly CourseDbContext _dbContext;

        public EnrollmentRepository(CourseDbContext dbContext) => _dbContext = dbContext;

        public async Task AddEnrollment(Enrollment enrollment)
        {
            await _dbContext.Enrollments.AddAsync(enrollment);
        }

        public void DeleteEnrollment(Enrollment enrollment)
        {
            _dbContext.Enrollments.Remove(enrollment);
        }

        public void DeleteEnrollmentsRange(List<Enrollment> enrollments)
        {
            _dbContext.Enrollments.RemoveRange(enrollments);
        }

        public async Task<List<long>> GetCourseUsersId(long courseId)
        {
            return await _dbContext.Enrollments.Where(d => d.CourseId == courseId && d.Active).Select(d => d.CustomerId).ToListAsync();
        }

        public async Task<List<Enrollment>?> GetEnrollmentsByUserIdAndCoursesIds(long userId, List<long> courses)
        {
            return await _dbContext.Enrollments.Where(d => d.CustomerId == userId && courses.Contains(d.CourseId)).ToListAsync();
        }

        public IPagedList<Enrollment> GetPagedEnrollments(long courseId, int page, int quantity)
        {
            var query = _dbContext.Enrollments.Where(d => d.CourseId == courseId && d.Active);

            return query.ToPagedList(page, quantity);
        }

        public async Task<List<Enrollment>?> UserEnrollments(long userId)
        {
            return await _dbContext.Enrollments.Include(d => d.Course).Where(d => d.CustomerId == userId).ToListAsync();
        }

        public async Task<bool> UserGotCourse(long courseId, long userId)
        {
            return await _dbContext.Enrollments.Where(d => d.CourseId == courseId && d.CustomerId == userId && d.Active)
                .SingleOrDefaultAsync() != null ? true : false;
        }
    }
}
