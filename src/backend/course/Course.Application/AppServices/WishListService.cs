using AutoMapper;
using Course.Communication.Responses;
using Course.Domain.Cache;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.AppServices
{
    public interface IWishListService
    {
        public Task<CourseWishListResponse> AddItemToWishList(long courseId, string sessionId);
        public Task<WishListResponse> GetUserWishList(string sessionId);
        public Task RemoveCourseFromWishList(long courseId, string sessionId);
    }

    public class WishListService : IWishListService
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserService _userService;
        private readonly ICourseCache _courseCache;

        public WishListService(IUnitOfWork uof, IMapper mapper, 
            SqidsEncoder<long> sqids, IUserService userService, ICourseCache courseCache)
        {
            _uof = uof;
            _mapper = mapper;
            _sqids = sqids;
            _userService = userService;
            _courseCache = courseCache;
        }

        public async Task<CourseWishListResponse> AddItemToWishList(long courseId, string sessionId)
        {
            var course = await _uof.courseRead.CourseById(courseId, true);

            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var response = new CourseWishListResponse();
            try
            {
                var user = await _userService.GetUserInfos();

                var userId = _sqids.Decode(user.id).Single();

                var wishList = new WishList() { CourseId = courseId, UserId = userId };

                await _uof.wishListWrite.Add(wishList);
                await _uof.Commit();
                response = _mapper.Map<CourseWishListResponse>(wishList);
            }
            catch (RestException ex)
            {
                await _courseCache.AddCourseToWishList(courseId, sessionId);
                response = new CourseWishListResponse() { CourseId = _sqids.Encode(courseId), UserId = sessionId };
            }

            return response;
        }

        public async Task<WishListResponse> GetUserWishList(string sessionId)
        {
            WishListResponse response = new WishListResponse();
            try
            {
                var user = await _userService.GetUserInfos();
                var userId = _sqids.Decode(user.id).Single();

                var wishList = await _uof.wishListRead.WishListByUserId(userId);
                response.UserId = user.id;
                response.Courses = wishList.Select(d =>
                {
                    var response = _mapper.Map<CourseWishListResponse>(wishList);

                    return response;
                }).ToList();
            }
            catch (RestException rex)
            {
                var wishListCache = await _courseCache.GetSessionWishList(sessionId);

                if (wishListCache is null)
                    return response;

                response.UserId = sessionId;
                response.Courses = wishListCache.Select(d =>
                {
                    var response = new CourseWishListResponse();
                    response.CourseId = _sqids.Encode(d);
                    response.UserId = sessionId;
                    return response;
                }).ToList();
            }
            return response;
        }

        public async Task RemoveCourseFromWishList(long courseId, string sessionId)
        {
            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            try
            {
                var user = await _userService.GetUserInfos();
                var userId = _sqids.Decode(user.id).Single();

                var wishList = await _uof.wishListRead.WishListByCourseAndUserId(userId, courseId);

                if (wishList is null)
                    throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

                _uof.wishListWrite.Delete(wishList);
                await _uof.Commit();
            }
            catch (RestException rex)
            {
                await _courseCache.RemoveCourseFromWishList(courseId, sessionId);
            }
        }
    }
}

