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

        public void DeleteCourseReviews(long courseId)
        {
            var reviews = _dbContext.Reviews.Where(d => d.CourseId == courseId);

            _dbContext.Reviews.RemoveRange(reviews);
        }

        public void DeleteReview(Review review)
        {
            _dbContext.Reviews.Remove(review);
        }

        public async Task<decimal> GetReviewSum(long courseId)
        {
            var reviews = _dbContext.Reviews.Where(d => d.CourseId == courseId && d.Active);

            return await reviews.SumAsync(d => (decimal)d.Rating);
        }

        public async Task<Review?> ReviewById(long id)
        {
            return await _dbContext.Reviews.SingleOrDefaultAsync(d => d.Id == id && d.Active);
        }

        public async Task<bool> ReviewOfUser(long userId, long reviewId)
        {
            return await _dbContext.Reviews.AnyAsync(d => d.CustomerId == userId && d.Id == reviewId && d.Active);
        }

        public async Task<List<Review>> ReviewsByCourse(long courseId)
        {
            return await _dbContext.Reviews.Where(d => d.Active && d.CourseId == courseId).ToListAsync();
        }

        public async Task<int> ReviewsCount(long courseId) => await _dbContext.Reviews
            .Where(d => d.CourseId == courseId && d.Active).CountAsync();

    }
}
