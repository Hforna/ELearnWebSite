using Course.Application.UseCases.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTestUtilities.Builds.Repositories;
using CommonTestUtilities.Builds.Services.Mapper;
using CommonTestUtilities.Builds.Services;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Course.Domain.Enums;
using Course.Exception;

namespace UseCases.Course
{
    public class CreateCourseUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var request = new CreateCourseRequestTest().Build();

            var useCase = CreateUseCase();
            var result = await useCase.Execute(request);

            result.Title.Should().Be(request.Title);
        }

        [Fact]
        public async Task FailCourseLanguageOutEnum()
        {
            var request = new CreateCourseRequestTest().Build();

            var useCase = CreateUseCase(true);
            Func<Task> result = async () => await useCase.Execute(request);

            await result.Should().ThrowAsync<UserException>(ResourceExceptMessages.USER_INFOS_DOESNT_EXISTS);
        }

        public CreateCourse CreateUseCase(bool isUserNull = false)
        {
            var uof = UnitOfWorkBuild.Build();
            var mapper = AutoMapperBuild.Build();
            var sqids = SqidsBuild.Build();
            var userService = new UserServiceBuild().Build(isUserNull);
            var storageService = StorageServiceBuild.Build();
            var fileService = FileServiceBuild.Build();

            var useCase = new CreateCourse(uof, mapper, userService, sqids, storageService, fileService);

            return useCase;
        }
    }
}
