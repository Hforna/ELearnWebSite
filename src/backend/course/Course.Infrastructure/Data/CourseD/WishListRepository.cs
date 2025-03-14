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

        public void Delete(WishList wishList)
        {
            _dbContext.WishList.Remove(wishList);
        }

        public async Task<WishList?> WishListByCourseAndUserId(long userId, long courseId)
        {
            return await _dbContext.WishList.SingleOrDefaultAsync(d => d.CourseId == courseId && d.UserId == userId && d.Active);
        }
    }
}
