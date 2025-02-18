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

        public CreateCourse CreateUseCase()
        {
            var uof = UnitOfWorkBuild.Build();
            var mapper = AutoMapperBuild.Build();
            var sqids = SqidsBuild.Build();
            var userService = new UserServiceBuild().Build();
            var storageService = StorageServiceBuild.Build();
            var fileService = FileServiceBuild.Build();

            var useCase = new CreateCourse(uof, mapper, userService, sqids, storageService, fileService);

            return useCase;
        }
    }
}
