using AutoMapper;
using Course.Application.UseCases.Repositories.WishLists;
using Course.Communication.Responses;
using Course.Domain.Cache;
using Course.Domain.Repositories;
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
    public class GetUserWishList : IGetUserWishList
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ICourseCache _courseCache;
        private readonly SqidsEncoder<long> _sqids;

        public GetUserWishList(IUnitOfWork uof, IUserService userService, 
            IMapper mapper, ICourseCache courseCache, SqidsEncoder<long> sqids)
        {
            _uof = uof;
            _userService = userService;
            _mapper = mapper;
            _courseCache = courseCache;
            _sqids = sqids;
        }

        public async Task<WishListResponse> Execute(string sessionId)
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
            }catch(RestException rex)
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
    }
}
