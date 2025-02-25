using CommonTestUtilities.Builds.Repositories;
using CommonTestUtilities.Builds.Services;
using CommonTestUtilities.Builds.Services.Mapper;
using CommonTestUtilities.Builds.Services.Rest;
using CommonTestUtilities.Entities;
using Course.Application.UseCases.Enrollments;
using Course.Domain.Entitites;
using Course.Exception;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Enrollment
{
    public class GetCourseEnrollmentsUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var course = CourseEntityTest.Build();
            var useCase = CreateUseCase(SqidsBuild.Build().Encode(course.TeacherId), course, 2, 2);
            var result = await useCase.Execute(course.Id, 2, 2);

            result.PageNumber.Should().Be(2);
            result.Enrollments.First().CourseId.Should().Be(SqidsBuild.Build().Encode(course.Id));
        }

        [Fact]
        public async Task CourseNotOfUser()
        {
            var course = CourseEntityTest.Build();
            var useCase = CreateUseCase(SqidsBuild.Build().Encode(course.TeacherId), course, 2, 2);
            course.TeacherId += 1;

            Func<Task> result = async () => await useCase.Execute(course.Id, 2, 2);

            await result.Should().ThrowAsync<CourseException>(ResourceExceptMessages.COURSE_NOT_OF_USER);
        }

        [Fact]
        public async Task CourseNotExists()
        {
            var course = CourseEntityTest.Build();
            var useCase = CreateUseCase(SqidsBuild.Build().Encode(course.TeacherId), course, 2, 2);

            Func<Task> result = async () => await useCase.Execute(course.Id + 1, 2, 2);

            await result.Should().ThrowAsync<CourseException>(ResourceExceptMessages.COURSE_DOESNT_EXISTS);
        }

        GetCourseEnrollments CreateUseCase(string userId, CourseEntity course, int page, int quantity)
        {
            var uof = UnitOfWorkBuild.Build();
            var courseRead = new CourseReadBuild();
            courseRead.CourseId(course);
            var enrollments = EnrollmentEntityTest.BuildEnrollmentRange(course);
            var enrollmentRead = new EnrollmentReadBuild();
            enrollmentRead.GetPagedEnrollments(page, quantity, course.Id, enrollments);

            var mapper = AutoMapperBuild.Build();
            var userService = new UserServiceBuild();
            var sqids = SqidsBuild.Build();
        
            return new GetCourseEnrollments(uof, mapper, userService.Build(false, userId), sqids);
        }
    }
}
