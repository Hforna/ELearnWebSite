using Course.Communication.Requests;
using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Reviews
{
    public interface ICreateReview
    {
        public Task<ReviewResponse> Execute(CreateReviewRequest request, long courseId);
    }
}
