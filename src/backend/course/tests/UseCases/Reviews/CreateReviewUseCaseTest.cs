using CommonTestUtilities.Builds.DTOs;
using CommonTestUtilities.Builds.Repositories;
using CommonTestUtilities.Builds.Services;
using CommonTestUtilities.Builds.Services.Mapper;
using CommonTestUtilities.Builds.Services.RabbitMq;
using CommonTestUtilities.Builds.Services.Rest;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Requests;
using Course.Application.UseCases.Reviews;
using Course.Domain.Entitites;
using Course.Exception;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Reviews
{
    public class CreateReviewUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var request = new CreateReviewRequestTest().Build(10);
            var course = CourseEntityTest.Build();
            var userId = 4;
            var useCase = CreateUseCase(course, 4);
            var result = await useCase.Execute(request, course.Id);

            result.CourseId.Should().Be(SqidsBuild.Build().Encode(course.Id));
            result.CustomerId.Should().Be(SqidsBuild.Build().Encode(userId));
        }

        [Fact]
        public async Task UserDoesntGotCourse()
        {
            var request = new CreateReviewRequestTest().Build(10);
            var course = CourseEntityTest.Build();
            var userId = 5;
            var useCase = CreateUseCase(course, userId, false);
            Func<Task> result = async () => await useCase.Execute(request, course.Id);

            await result.Should().ThrowAsync<CourseException>(ResourceExceptMessages.USER_DOESNT_GOT_COURSE);
        }

        CreateReview CreateUseCase(CourseEntity course, long userId, bool userGotCourse = true)
        {
            var uof = UnitOfWorkBuild.Build();

            var enrollmentRead = new EnrollmentReadBuild();
            enrollmentRead.UserGotCourse(course.Id, userId, userGotCourse);
            uof.enrollmentRead = enrollmentRead.Build();

            var reviewRead = new ReviewReadBuild();
            reviewRead.GetReviewCount(course, 10);
            reviewRead.GetReviewsSum(course, 26);

            var courseRead = new CourseReadBuild();
            courseRead.CourseId(course);
            uof.courseRead = courseRead.Build();

            var mapper = AutoMapperBuild.Build();
            var sqids = SqidsBuild.Build();
            var user = new UserServiceBuild().Build(userId: sqids.Encode(userId));
            var userSender = UserSenderServiceBuild.Build();


            return new CreateReview(uof, mapper, user, sqids, userSender);
        }
    }
}
