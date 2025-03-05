using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Reviews
{
    public interface IDeleteReview
    {
        public Task Execute(long reviewId);
    }
}
