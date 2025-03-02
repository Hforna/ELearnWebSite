using CommonTestUtilities.Builds.Repositories;
using CommonTestUtilities.Builds.Services;
using CommonTestUtilities.Builds.Services.Mapper;
using CommonTestUtilities.Builds.Services.Rest;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Requests;
using Course.Application.UseCases.Modules;
using Course.Domain.Entitites;
using Course.Exception;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Modules
{
    public class UpdateModuleUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var request = UpdateModuleRequestTest.Build(2);
            var course = CourseEntityTest.Build();
            var modules = new ModuleEntityTest().CreateModules(10, course.Id);
            course.Modules = modules;
            var teacherIdsqids = SqidsBuild.Build().Encode(course.TeacherId);
            var useCase = CreateUseCase(course, teacherIdsqids);
            var result = await useCase.Execute(course.Id, modules.FirstOrDefault()!.Id,  request);

            result.Description.Should().Be(request.Description);

        }

        [Fact]
        public async Task CourseNotOfUser()
        {
            var request = UpdateModuleRequestTest.Build(2);
            var course = CourseEntityTest.Build();
            var modules = new ModuleEntityTest().CreateModules(10, course.Id);
            course.Modules = modules;
            var teacherIdsqids = SqidsBuild.Build().Encode(course.TeacherId);
            var useCase = CreateUseCase(course, SqidsBuild.Build().Encode(5));
            Func<Task> result = async () => await useCase.Execute(course.Id, modules.FirstOrDefault()!.Id, request);

            await result.Should().ThrowAsync<CourseException>(ResourceExceptMessages.COURSE_NOT_OF_USER);
        }

        UpdateModule CreateUseCase(CourseEntity course, string userId)
        {
            var uof = UnitOfWorkBuild.Build();
            var courseRead = new CourseReadBuild();
            courseRead.CourseId(course);
            uof.courseRead = courseRead.Build();

            var mapper = AutoMapperBuild.Build();
            var userService = new UserServiceBuild().Build(userId: userId);
            var sqids = SqidsBuild.Build();

            return new UpdateModule(uof, mapper, userService, sqids);
        }
    }
}
