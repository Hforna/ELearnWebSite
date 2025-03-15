using AutoMapper;
using Course.Application.UseCases.Repositories.WishLists;
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.WishLists
{
    public class AddItemToWishList : IAddItemToWishList
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserService _userService;
        private readonly ICourseCache _courseCache;

        public AddItemToWishList(IUnitOfWork uof, IMapper mapper, 
            SqidsEncoder<long> sqids, IUserService userService, ICourseCache courseCache)
        {
            _uof = uof;
            _mapper = mapper;
            _sqids = sqids;
            _userService = userService;
            _courseCache = courseCache;
        }

        public async Task<WishListResponse> Execute(long courseId, string sessionId)
        {
            var course = await _uof.courseRead.CourseById(courseId, true);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            var response = new WishListResponse();
            try
            {
                var user = await _userService.GetUserInfos();

                var userId = _sqids.Decode(user.id).Single();

                var wishList = new WishList() { CourseId = courseId, UserId = userId };

                await _uof.wishListWrite.Add(wishList);
                await _uof.Commit();
                response = _mapper.Map<WishListResponse>(wishList);
            } catch(RestException ex)
            {
                await _courseCache.AddCourseToWishList(courseId, sessionId);
                response = new WishListResponse() { CourseId = _sqids.Encode(courseId), UserId = sessionId};
            }

            return response;
        }
    }
}
