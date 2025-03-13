using CommonTestUtilities.Builds.Repositories;
using CommonTestUtilities.Builds.Services;
using CommonTestUtilities.Builds.Services.RabbitMq;
using CommonTestUtilities.Builds.Services.Rest;
using CommonTestUtilities.Entities;
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
    public class DeleteReviewUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var course = CourseEntityTest.Build();
            var review = ReviewEntityTest.Build(course.Id, 5);
            var useCase = CreateUseCase(course, 5, review);
            Func<Task> result = async () => await useCase.Execute(review.Id);

            await result.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ReviewNotOfUser()
        {
            var course = CourseEntityTest.Build();
            var review = ReviewEntityTest.Build(course.Id, 5);
            var useCase = CreateUseCase(course, 2, review, false);
            Func<Task> result = async () => await useCase.Execute(review.Id);

            await result.Should().ThrowAsync<ReviewException>(ResourceExceptMessages.REVIEW_NOT_OF_USER);
        }

        DeleteReview CreateUseCase(CourseEntity course, long userId, Review review, bool reviewOfUser = true)
        {
            var reviewRead = new ReviewReadBuild();
            reviewRead.ReviewById(review);
            reviewRead.GetReviewCount(course, 10);
            reviewRead.GetReviewsSum(course, 26);
            reviewRead.ReviewOfUser(review.Id, userId, reviewOfUser);
            var courseRead = new CourseReadBuild();
            courseRead.CourseId(course);

            var userService = new UserServiceBuild();
            var sqids = SqidsBuild.Build();

            var uof = UnitOfWorkBuild.Build(reviewReadMock: reviewRead.Build(), courseReadMock: courseRead.Build());
            var userSender = UserSenderServiceBuild.Build();

            return new DeleteReview(uof, userService.Build(userId: sqids.Encode(userId)), sqids, userSender);
        }
    }
}
