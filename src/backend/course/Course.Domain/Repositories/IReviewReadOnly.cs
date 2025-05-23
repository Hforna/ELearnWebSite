﻿using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IReviewReadOnly
    {
        public Task<List<Review>> ReviewsByCourse(long courseId);
        public Task<Review?> ReviewById(long id);
        public Task<bool> ReviewOfUser(long userId, long reviewId);
        public Task<List<Review>?> UserReviews(long userId, long courseId);
        public Task<decimal> GetReviewSum(long courseId);
        public Task<int> ReviewsCount(long courseId);
    }
}
