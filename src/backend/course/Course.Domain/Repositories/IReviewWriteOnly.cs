using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IReviewWriteOnly
    {
        public void Add(Review review);
        public void DeleteReview(Review review);
        public void DeleteCourseReviews(long courseId);
        public void DeleteReviewsRange(List<Review> reviews);
        public Task AddReviewResponse(ReviewResponseEntity reviewResponse);
    }
}
