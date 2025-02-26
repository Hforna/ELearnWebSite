using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Infrastructure.Data.Course;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data.CourseD
{
    public class ReviewRepository : IReviewReadOnly, IReviewWriteOnly
    {
        private readonly CourseDbContext _dbContext;

        public ReviewRepository(CourseDbContext dbContext) => _dbContext = dbContext;

        public void Add(Review review)
        {
            _dbContext.Reviews.Add(review);
        }

        public async Task<decimal> GetReviewSum(long courseId)
        {
            var reviews = _dbContext.Reviews.Where(d => d.CourseId == courseId && d.Active);

            return await reviews.SumAsync(d => (decimal)d.Rating);
        }

        public async Task<List<Review>> ReviewsByCourse(long courseId)
        {
            return await _dbContext.Reviews.Where(d => d.Active && d.CourseId == courseId).ToListAsync();
        }

        public async Task<int> ReviewsCount(long courseId) => await _dbContext.Reviews
            .Where(d => d.CourseId == courseId && d.Active).CountAsync();

    }
}
