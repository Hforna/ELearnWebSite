using AutoMapper;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Cache;
using Course.Domain.DTOs;
using Course.Domain.Entitites;
using Course.Domain.Enums;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Domain.Sessions;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Course
{
    public class GetCourses : IGetCourses
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly ICoursesSession _coursesSession;
        private readonly ICourseCache _courseCache;
        private readonly ILocationService _locationService;
        private readonly ICurrencyExchangeService _exchangeService;

        public GetCourses(IUnitOfWork uof, IMapper mapper, 
            IStorageService storageService, SqidsEncoder<long> sqids, 
            ICoursesSession coursesSession, ICourseCache courseCache, ILocationService locationService, ICurrencyExchangeService exchangeService)
        {
            _uof = uof;
            _courseCache = courseCache;
            _mapper = mapper;
            _exchangeService = exchangeService;
            _locationService = locationService;
            _storageService = storageService;
            _sqids = sqids;
            _coursesSession = coursesSession;
        }

        public async Task<CoursesPaginationResponse> Execute(GetCoursesRequest request, int page, int itemsQuantity)
        {
            var filterDto = _mapper.Map<GetCoursesFilterDto>(request);

            var getCoursesList = _coursesSession.GetCoursesVisited();

            List<string>? coursesTypes = null;

            if(getCoursesList is not null)
            {
                var visitedCourses = await _uof.courseRead.CourseByIds(getCoursesList);
                coursesTypes = visitedCourses!
                    .GroupBy(d => d.CourseType)
                    .ToDictionary(d => d.Key.ToString(), g => g.Count())
                    .OrderByDescending(d => d.Value)
                    .Select(d => d.Key).ToList();
            }
            var mostPopularCourses = await _courseCache.GetMostPopularCourses();
            List<CourseEntity>? reccomendedCourses = new List<CourseEntity>();

            if(mostPopularCourses is not null)
            {
                mostPopularCourses = mostPopularCourses.OrderByDescending(d => d.Value).ToDictionary();

                reccomendedCourses = await _uof.courseRead.GetCourseByUserVisitsAndMostVisited(filterDto, mostPopularCourses.Keys.ToList(), coursesTypes);
            }

            var courses = _uof.courseRead.GetCoursesPagination(page, filterDto, reccomendedCourses, itemsQuantity);

            var userCurrency = await _locationService.GetCurrencyByUserLocation();
            var userCurrencyAsEnum = Enum.Parse(typeof(CurrencyEnum), userCurrency.Code);

            var coursesToResponse = courses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                //response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);
                var ratesByCourseCurrency = await _exchangeService.GetCurrencyRates(course.CurrencyType);
                var convertPrice = course.Price;

                switch (userCurrencyAsEnum)
                {
                    case CurrencyEnum.BRL:
                        convertPrice = course.Price * ratesByCourseCurrency.BRL;
                        break;
                    case CurrencyEnum.USD:
                        convertPrice = course.Price * ratesByCourseCurrency.USD;
                        break;
                    case CurrencyEnum.EUR:
                        convertPrice = course.Price * ratesByCourseCurrency.EUR;
                        break;
                }
                response.Price = Math.Round(convertPrice, 2, MidpointRounding.AwayFromZero);

                return response;
            });

            var coursesResponse = await Task.WhenAll(coursesToResponse);

            var response = new CoursesPaginationResponse()
            {
                Count = courses.Count,
                Courses = coursesResponse.ToList(),
                IsFirstPage = courses.IsFirstPage,
                IsLastPage = courses.IsLastPage,
                PageNumber = courses.PageNumber,
                TotalItemCount = courses.TotalItemCount
            };

            return response;
        }
    }
}
