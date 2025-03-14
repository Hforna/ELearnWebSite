using Course.Application.UseCases.Repositories.WishLists;
using Course.Domain.Cache;
using Course.Domain.Repositories;
using Course.Domain.Services.RabbitMq;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.WishLists
{
    public class RemoveCourseFromWishList : IRemoveCourseFromWishList
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserService _userService;
        private readonly ICourseCache _courseCache;
        private readonly SqidsEncoder<long> _sqids;

        public RemoveCourseFromWishList(IUnitOfWork uof, IUserService userService, 
            ICourseCache courseCache, SqidsEncoder<long> sqids)
        {
            _uof = uof;
            _userService = userService;
            _courseCache = courseCache;
            _sqids = sqids;
        }

        public async Task Execute(long courseId, string sessionId)
        {
            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            try
            {
                var user = await _userService.GetUserInfos();
                var userId = _sqids.Decode(user.id).Single();

                var wishList = await _uof.wishListRead.WishListByCourseAndUserId(userId, courseId);

                if (wishList is null)
                    throw new WishListException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

                _uof.wishListWrite.Delete(wishList);
                await _uof.Commit();
            } catch(RestException rex)
            {
                await _courseCache.RemoveCourseFromWishList(courseId, sessionId);
            }
        }
    }
}
