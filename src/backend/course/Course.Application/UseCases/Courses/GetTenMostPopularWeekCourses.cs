using AutoMapper;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Responses;
using Course.Domain.Cache;
using Course.Domain.Enums;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Courses
{
    public class GetTenMostPopularWeekCourses : IGetTenMostPopularWeekCourses
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly ICourseCache _courseCache;
        private readonly IStorageService _storageService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly ILocationService _locationService;
        private readonly ICurrencyExchangeService _exchangeService;

        public GetTenMostPopularWeekCourses(IMapper mapper, IUnitOfWork uof, 
            ICourseCache courseCache, IStorageService storageService, 
            SqidsEncoder<long> sqids, ILocationService locationService, ICurrencyExchangeService exchangeService)
        {
            _mapper = mapper;
            _uof = uof;
            _locationService = locationService;
            _exchangeService = exchangeService;
            _courseCache = courseCache;
            _storageService = storageService;
            _sqids = sqids;
        }

        public async Task<CoursesResponse> Execute()
        {
            var cacheCourses = await _courseCache.GetMostPopularCourses();
            if (cacheCourses is null)
                return new CoursesResponse();

            cacheCourses = cacheCourses.Take(10).ToDictionary(k => k.Key, v => v.Value);
            
            var courses = await _uof.courseRead.CourseByIds(cacheCourses.Keys.ToList());

            var userCurrency = await _locationService.GetCurrencyByUserLocation();
            var userCurrencyAsEnum = Enum.Parse(typeof(CurrencyEnum), userCurrency.Code);

            var courseTasks = courses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);
                var courseRates = await _exchangeService.GetCurrencyRates(course.CurrencyType);
                var convertPrice = course.Price;

                switch(userCurrencyAsEnum)
                {
                    case CurrencyEnum.BRL:
                        convertPrice = course.Price * courseRates.BRL;
                        break;
                    case CurrencyEnum.USD:
                        convertPrice = course.Price * courseRates.USD;
                        break;
                    case CurrencyEnum.EUR:
                        convertPrice = course.Price * courseRates.EUR;
                        break;
                }
                response.Price = Math.Round(convertPrice, 2, MidpointRounding.AwayFromZero);

                return response;
            }).ToList();

            var taskCourseResponse = await Task.WhenAll(courseTasks);

            return new CoursesResponse { courses = taskCourseResponse.ToList() };
        }
    }
}
