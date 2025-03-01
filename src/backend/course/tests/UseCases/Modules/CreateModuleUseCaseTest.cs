using CommonTestUtilities.Builds.Repositories;
using CommonTestUtilities.Builds.Services;
using CommonTestUtilities.Builds.Services.Azure;
using CommonTestUtilities.Builds.Services.Mapper;
using CommonTestUtilities.Builds.Services.Rest;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Requests;
using Course.Application.UseCases.Modules;
using Course.Domain.Entitites;
using Course.Exception;
using FluentAssertions;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Modules
{
    public class CreateModuleUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var request = CreateModuleRequestTest.Build(1);
            var course = CourseEntityTest.Build();
            course.Modules = [];
            course.TeacherId = 5;
            var useCase = CreateUseCase(5, course);
            Func<Task> result = async () => await useCase.Execute(request, course.Id);

            await result.Should().NotThrowAsync();
        }

        [Fact]
        public async Task PositionOutRange()
        {
            var course = CourseEntityTest.Build();
            course.TeacherId = 5;
            var modules = new ModuleEntityTest().CreateModules(5, course.Id);
            course.Modules = modules;
            var request = CreateModuleRequestTest.Build(course.Modules.Count + 2);
            var useCase = CreateUseCase(5, course);
            Func<Task> result = async () => await useCase.Execute(request, course.Id);

            await result.Should().ThrowAsync<CourseException>(ResourceExceptMessages.MODULE_POSITION_OUT_RANGE);
        }

        CreateModule CreateUseCase(long userId, CourseEntity course)
        {
            var uof = UnitOfWorkBuild.Build();
            var courseRead = new CourseReadBuild();
            courseRead.CourseByTeacherAndIdBuild(course);
            uof.courseRead = courseRead.Build();
            var mapper = AutoMapperBuild.Build();
            var sqids = SqidsBuild.Build();
            var userService = new UserServiceBuild();
            var linkService = LinkServiceBuild.Build("api/module", course.Id.ToString());
            var moduleSender = NewModuleSenderBuild.Build();

            return new CreateModule(uof, mapper, sqids, userService.Build(false, sqids.Encode(userId)), linkService, moduleSender);
        }
    }
}
