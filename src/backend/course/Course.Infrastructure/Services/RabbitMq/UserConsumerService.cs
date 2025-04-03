using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using MassTransit;
using SharedMessages.UserMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.RabbitMq
{
    public class UserConsumerService : IConsumer<UserDeletedMessage>
    {
        private readonly IUnitOfWork _uow;
        private readonly IStorageService _storageService;

        public UserConsumerService(IUnitOfWork uow, IStorageService storageService)
        {
            _uow = uow;
            _storageService = storageService;
        }

        public async Task Consume(ConsumeContext<UserDeletedMessage> context)
        {
            var userId = context.Message.UserId;

            var userWishList = await _uow.wishListRead.WishListByUserId(userId);

            if(userWishList is not null)
            {
                foreach(var wishList in userWishList)
                {
                    _uow.wishListWrite.Delete(wishList);
                }
            }

            var enrollments = await _uow.enrollmentRead.UserEnrollments(userId);

            if(enrollments is not null)
            {
                foreach(var enrollment in enrollments)
                {
                    var course = enrollment.Course;
                    course.Enrollments -= 1;

                    _uow.courseWrite.UpdateCourse(course);

                    _uow.enrollmentWrite.DeleteEnrollment(enrollment);
                }
            }

            if(context.Message.Teacher)
            {
                var courses = await _uow.courseRead.CoursesByTeacher(userId);

                if(courses is not null)
                {
                    foreach(var course in courses)
                    {
                        await _storageService.DeleteCourseImage(course.courseIdentifier, course.Thumbnail);
                        await _storageService.DeleteContainer(course.courseIdentifier);

                        var modules = course.Modules;
                        var videoIds = new List<string>();

                        foreach(var module in modules)
                        {
                            videoIds.AddRange(module.Lessons.Select(d => d.VideoId).ToList());
                        }
                        await _uow.videoWrite.DeleteRangeVideos(videoIds);
                    }
                    _uow.courseWrite.DeleteCourseRange(courses);
                }
            }

            await _uow.Commit();
        }
    }
}
