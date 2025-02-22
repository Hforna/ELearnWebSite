using CommonTestUtilities.Builds.Repositories;
using CommonTestUtilities.Builds.Services.Mapper;
using CommonTestUtilities.Builds.Services;
using Course.Application.UseCases.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Course
{
    public class GetCourseUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            
        }

        GetCourse CreateUseCase()
        {
            var uof = UnitOfWorkBuild.Build();
            var mapper = AutoMapperBuild.Build();
            var sqids = SqidsBuild.Build();
            var userService = new UserServiceBuild().Build();
            var storageService = StorageServiceBuild.Build();
            var fileService = FileServiceBuild.Build();


            var useCase = new GetCourse(uof, mapper, sqids, storageService, );

            return useCase;
        }
    }
}
