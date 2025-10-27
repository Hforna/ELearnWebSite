using Course.Domain.Cache;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using System.Text.Json;
using StackExchange.Redis;
using Course.Exception;

namespace Course.Api.Cache
{
    public class CourseCache : ICourseCache
    {
        private readonly IDistributedCache _redis;
        private readonly TimeSpan _cacheCourseExpire = TimeSpan.FromDays(7);
        private readonly TimeSpan _cacheWishListExpire = TimeSpan.FromDays(20);

        public CourseCache(IDistributedCache redis)
        {
            _redis = redis;
        }

        public async Task AddCourseToWishList(long courseId, string sessionId)
        {
            var wishList = await _redis.GetAsync($"{sessionId}_wish_list");

            var deserializeList = new List<long>();

            if(wishList is not null && wishList.Length > 1)
            {
                deserializeList = JsonSerializer.Deserialize<List<long>>(wishList);

                if (deserializeList.Contains(courseId))
                    throw new UnauthorizedException(ResourceExceptMessages.COURSE_IN_WISH_LIST);
            }
            deserializeList.Add(courseId);

            var serializeList = JsonSerializer.SerializeToUtf8Bytes(deserializeList);

            await _redis.SetAsync(sessionId, serializeList, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = _cacheWishListExpire
            });
        }

        public async Task<Dictionary<long, int>?> GetMostPopularCourses()
        {
            var weekCourses = await _redis.GetStringAsync("most_popular_week_courses");

            if (string.IsNullOrEmpty(weekCourses))
                return null;

            var deserializeList = JsonSerializer.Deserialize<List<KeyValuePair<long, int>>>(weekCourses);
            return deserializeList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public async Task<List<long>?> GetSessionWishList(string sessionId)
        {
            var wishList = await _redis.GetAsync($"{sessionId}_wish_list");

            if (wishList is null || wishList.Length < 1)
                return null;

            var deserializeList = JsonSerializer.Deserialize<List<long>>(wishList);

            return deserializeList;
        }

        public async Task RemoveCourseFromWishList(long courseId, string sessionId)
        {
            var wishList = await _redis.GetAsync($"{sessionId}_wish_list");

            if (wishList is null)
                return;

            var deserializeList = JsonSerializer.Deserialize<List<long>>(wishList);

            if (deserializeList.Contains(courseId) == false)
                return;

            deserializeList.Remove(courseId);
            var serializeList = JsonSerializer.SerializeToUtf8Bytes(deserializeList);

            await _redis.SetAsync($"{sessionId}_wish_list", serializeList, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = _cacheWishListExpire
            });
        }

        public async Task SetCourseOnMostVisited(long courseId)
        {
            string keyString = "most_popular_week_courses";

            var weekCourses = await _redis.GetStringAsync(keyString);
            string serializeDict;
            Dictionary<long, int> weekCoursesDict = new Dictionary<long, int>();

            if (string.IsNullOrEmpty(weekCourses))
            {
                weekCoursesDict[courseId] = 1;
            } else
            {
                var deserializeKvp = JsonSerializer.Deserialize<List<KeyValuePair<long, int>>>(weekCourses);
                weekCoursesDict = deserializeKvp.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                weekCoursesDict[courseId] = weekCoursesDict.ContainsKey(courseId)
                    ? weekCoursesDict[courseId] + 1 : 1;
            }

            serializeDict = JsonSerializer.Serialize(weekCoursesDict.OrderByDescending(d => d.Value));

            await _redis.SetStringAsync(keyString, serializeDict, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = _cacheCourseExpire
            });
        }
    }
}
