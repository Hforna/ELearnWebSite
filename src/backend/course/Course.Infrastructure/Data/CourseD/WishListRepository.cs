using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Infrastructure.Data.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data.CourseD
{
    public class WishListRepository : IWishListReadOnly, IWishListWriteOnly
    {
        private readonly CourseDbContext _dbContext;

        public WishListRepository(CourseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(WishList wishList)
        {
            await _dbContext.WishList.AddAsync(wishList);
        }
    }
}
