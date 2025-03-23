using CommonTestUtilities.Builds.Repositories;
using CommonTestUtilities.Builds.Services.Cache;
using CommonTestUtilities.Builds.Services.Mapper;
using CommonTestUtilities.Builds.Services.Rest;
using CommonTestUtilities.Builds.Services.Session;
using CommonTestUtilities.Builds.Services;
using Course.Application.UseCases.Course;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Entities;
using Course.Domain.DTOs;
using Course.Domain.Entitites;
using FluentAssertions;
using Course.Domain.Sessions;
using CommonTestUtilities.Builds.DTOs;

namespace UseCases.Course
{
    public class GetCoursesUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var request = new GetCoursesRequestTest().Build();
            var ids = new List<long>() { 1, 2, 10, 59, 28, 67, 30 };
            var courses = CourseEntityTest.CreateRangeCourse(ids, 10);

            var useCase = CreateUseCase(ids, ids[4], AutoMapperBuild.Build().Map<GetCoursesFilterDto>(request), 1, 2, courses);
            var result = await useCase.Execute(request, 1, 2);

            result.IsFirstPage.Should().Be(true);
        }

        GetCourses CreateUseCase(List<long> ids, long courseId, GetCoursesFilterDto filterDto, 
            int page, int quantityItems, List<CourseEntity> courses)
        {
            var uof = UnitOfWorkBuild.Build();
            var courseRead = new CourseReadBuild();
            courseRead.GetCourseByUserVisitsAndMostVisited(filterDto, ids, courses);
            (List<long> popularIds, ICoursesSession courseSession) = CourseSessionBuild.Build(courseId);
            courseRead.GetCoursesPagination(page, quantityItems, filterDto, courses, courses);
            courseRead.CourseByIds(popularIds, CourseEntityTest.CreateRangeCourse(popularIds));

            uof.courseRead = courseRead.Build();
            var mapper = AutoMapperBuild.Build();
            var sqids = SqidsBuild.Build();
            var userService = new UserServiceBuild().Build();
            var storageService = StorageServiceBuild.Build();
            var fileService = FileServiceBuild.Build();;
            var cache = new CourseCacheBuild();
            cache.GetMostPopularCourses(courseId, 5);
            var linkService = LinkServiceBuild.Build();
            var locationService = new LocationServiceBuild();
            var locationDto = CurrencyByLocationDtoTest.Build();
            locationService.CurrencyByUserLocation(locationDto);

            var currencyExchange = new CurrencyExchangeServiceBuild();
            var ratesExchangeDto = RateExchangeDtoTest.Build();
            currencyExchange.CurrencyRates(ratesExchangeDto);

            return new GetCourses(uof, mapper, storageService, sqids, 
                courseSession, cache.Build(), locationService.Build(), currencyExchange.Build());
        }
    }

    
}
