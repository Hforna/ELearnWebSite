using Course.Domain.Cache;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using System.Text.Json;
using StackExchange.Redis;

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
            var wishList = await _redis.GetAsync(sessionId);

            var deserializeList = new List<long>();

            if(wishList is not null || wishList.Any())
            {
                deserializeList = JsonSerializer.Deserialize<List<long>>(wishList);
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

            var deserializeList = JsonSerializer.Deserialize<Dictionary<long, int>>(weekCourses);
            return deserializeList;
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
                weekCoursesDict = JsonSerializer.Deserialize<Dictionary<long, int>>(weekCourses)!;
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
