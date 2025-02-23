using CommonTestUtilities.Builds.Repositories;
using CommonTestUtilities.Builds.Services.Mapper;
using CommonTestUtilities.Builds.Services;
using CommonTestUtilities.Requests;
using Course.Application.UseCases.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Course.Exception;
using Course.Domain.Entitites;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Builds.Services.Rest;

namespace UseCases.Course
{
    public class UpdateCourseUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var request = new UpdateCourseRequestTest().Build();
            var id = SqidsBuild.GenerateRandomLong();
            var course = CourseEntityTest.Build(id);

            var useCase = CreateUseCase(course);
            Func<Task> result = async () => await useCase.Execute(id, request);

            await result.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ResultsEncodedSuccess()
        {
            var request = new UpdateCourseRequestTest().Build();
            var id = SqidsBuild.GenerateRandomLong();
            var decodedId = SqidsBuild.Build().Encode(id);
            var course = CourseEntityTest.Build(id);

            var useCase = CreateUseCase(course);
            var result = await useCase.Execute(id, request);

            result.CourseId.Should().Be(decodedId);
        }

        [Fact]
        public async Task CourseNotExists()
        {
            var request = new UpdateCourseRequestTest().Build();
            var id = SqidsBuild.GenerateRandomLong();
            var course = CourseEntityTest.Build(id);

            var useCase = CreateUseCase(course, returnCourseNull: true);
            Func<Task> result = async () => await useCase.Execute(id, request);

            await result.Should().ThrowAsync<CourseException>(ResourceExceptMessages.COURSE_DOESNT_EXISTS);
        }
     

        UpdateCourse CreateUseCase(CourseEntity course, bool returnCourseNull = false)
        {
            var uof = UnitOfWorkBuild.Build();
            var mapper = AutoMapperBuild.Build();
            var sqids = SqidsBuild.Build();
            var userService = new UserServiceBuild().Build(userId: SqidsBuild.Build().Encode(course.TeacherId));
            var storageService = StorageServiceBuild.Build();
            var fileService = FileServiceBuild.Build();
            var courseRead = new CourseReadBuild();
            courseRead.CourseByTeacherAndIdBuild(course!, returnCourseNull);
            uof.courseRead = courseRead.Build();

            return new UpdateCourse(uof, mapper, storageService, userService, sqids, fileService);
        }
    }
}
