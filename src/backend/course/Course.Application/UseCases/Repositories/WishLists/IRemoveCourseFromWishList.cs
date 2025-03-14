using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.WishLists
{
    public interface IRemoveCourseFromWishList
    {
        public Task Execute(long courseId, string sessionId);
    }
}
