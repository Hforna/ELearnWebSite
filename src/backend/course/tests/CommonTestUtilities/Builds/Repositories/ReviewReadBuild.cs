using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Repositories
{
    public class ReviewReadBuild
    {
        private Mock<IReviewReadOnly> _mock = new Mock<IReviewReadOnly>();

        public IReviewReadOnly Build() => _mock.Object;

        public void GetReviewsSum(CourseEntity course, int sumReview)
        {
            _mock.Setup(d => d.GetReviewSum(course.Id)).ReturnsAsync(sumReview);
        }

        public void ReviewById(Review review)
        {
            _mock.Setup(d => d.ReviewById(review.Id)).ReturnsAsync(review);
        }

        public void ReviewOfUser(long reviewId, long userId, bool reviewOfuser = true)
        {
            _mock.Setup(d => d.ReviewOfUser(userId, reviewId)).ReturnsAsync(reviewOfuser);
        }

        public void GetReviewCount(CourseEntity course, int countReview)
        {
            _mock.Setup(d => d.ReviewsCount(course.Id)).ReturnsAsync(countReview);
        }
    }
}
