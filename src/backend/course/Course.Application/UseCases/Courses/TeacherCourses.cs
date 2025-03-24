using AutoMapper;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Responses;
using Course.Domain.Enums;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Courses
{
    public class TeacherCourses : ITeacherCourses
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IStorageService _storageService;
        private readonly ILocationService _locationService;
        private readonly ICurrencyExchangeService _currencyExchange;

        public TeacherCourses(IMapper mapper, IUnitOfWork uof, 
            SqidsEncoder<long> sqids, IStorageService storageService, 
            ILocationService locationService, ICurrencyExchangeService currencyExchange)
        {
            _mapper = mapper;
            _uof = uof;
            _sqids = sqids;
            _locationService = locationService;
            _currencyExchange = currencyExchange;
            _storageService = storageService;
        }

        public async Task<CoursesPaginationResponse> Execute(int page, int quantity, long teacherId)
        {
            var courses = _uof.courseRead.TeacherCoursesPagination(page, quantity, teacherId);

            if (courses is null)
                throw new CourseException(ResourceExceptMessages.TEACHER_DOESNT_HAVE_COURSES, System.Net.HttpStatusCode.NotFound);

            var userCurrency = await _locationService.GetCurrencyByUserLocation();
            var userCurrencyAsEnum = Enum.Parse(typeof(CurrencyEnum), userCurrency.Code);

            var coursesResponse = courses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);
                var courseRates = await _currencyExchange.GetCurrencyRates(course.CurrencyType);
                var convertPrice = course.Price;

                switch (userCurrencyAsEnum)
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
            });

            var courseResponseList = await Task.WhenAll(coursesResponse);

            return new CoursesPaginationResponse()
            {
                Count = courses.Count,
                Courses = courseResponseList.ToList(),
                IsFirstPage = courses.IsFirstPage,
                IsLastPage = courses.IsLastPage,
                PageNumber = courses.PageNumber,
                TotalItemCount = courses.TotalItemCount
            };
        }
    }
}
