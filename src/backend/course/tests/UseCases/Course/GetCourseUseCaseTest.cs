using CommonTestUtilities.Builds.Repositories;
using CommonTestUtilities.Builds.Services.Mapper;
using CommonTestUtilities.Builds.Services;
using Course.Application.UseCases.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTestUtilities.Builds.Services.Cache;
using CommonTestUtilities.Builds.Services.Rest;
using CommonTestUtilities.Builds.Services.Session;
using Course.Domain.Entitites;
using CommonTestUtilities.Entities;
using FluentAssertions;
using Course.Exception;

namespace UseCases.Course
{
    public class GetCourseUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var course = CourseEntityTest.Build();
            var useCase = CreateUseCase(5, 10, course);
            var result = await useCase.Execute(course.Id);
            var sqids = SqidsBuild.Build().Encode(course.Id);

            result.Id.Should().Be(sqids);
        }

        [Fact]
        public async Task CourseNull()
        {
            var course = CourseEntityTest.Build();
            var useCase = CreateUseCase(5, 10, course, true);
            Func<Task> result = async () => await useCase.Execute(course.Id);

            await result.Should().ThrowAsync<CourseException>(ResourceExceptMessages.COURSE_DOESNT_EXISTS);
        }

        GetCourse CreateUseCase(long courseId, int visits, CourseEntity course, bool returnNull = false)
        {
            var uof = UnitOfWorkBuild.Build();
            var courseRead = new CourseReadBuild();
            courseRead.CourseId(course, returnNull);
            uof.courseRead = courseRead.Build();
            var mapper = AutoMapperBuild.Build();
            var sqids = SqidsBuild.Build();
            var userService = new UserServiceBuild().Build();
            var storageService = StorageServiceBuild.Build();
            var fileService = FileServiceBuild.Build();
            var cache = new CourseCacheBuild();
            cache.GetMostPopularCourses(courseId, visits);
            var linkService = LinkServiceBuild.Build();
            var sessionService = CourseSessionBuild.Build(courseId);

            var useCase = new GetCourse(uof, mapper, sqids, storageService, sessionService.Item2, linkService, cache.Build());

            return useCase;
        }
    }
}
