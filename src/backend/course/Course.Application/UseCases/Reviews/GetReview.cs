using AutoMapper;
using Course.Application.UseCases.Repositories.Reviews;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Reviews
{
    public class GetReview : IGetReview
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public GetReview(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        public async Task<ReviewResponse> Execute(long reviewId)
        {
            var review = await _uof.reviewRead.ReviewById(reviewId);

            if (review is null)
                throw new ReviewException(ResourceExceptMessages.REVIEW_NOT_EXISTS);

            var response = _mapper.Map<ReviewResponse>(review);

            return response;
        }
    }
}
