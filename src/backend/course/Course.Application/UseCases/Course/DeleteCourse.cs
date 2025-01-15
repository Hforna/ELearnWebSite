using Course.Application.Services;
using Course.Application.UseCases.Repositories.Course;
using Course.Domain.DTOs;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Course
{
    public class DeleteCourse : IDeleteCourse
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;
        private readonly EmailService _emailService;

        public DeleteCourse(IUserService userService, IUnitOfWork uof, SqidsEncoder<long> sqids, EmailService emailService)
        {
            _userService = userService;
            _uof = uof;
            _sqids = sqids;
            _emailService = emailService;
        }

        public async Task Execute(long id)
        {
            var course = await _uof.courseRead.CourseById(id);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            UserInfosDto user = await _userService.GetUserInfos();
            long userId = _sqids.Decode(user.id).Single();

            if (course.TeacherId != userId)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_OF_USER);

            course.Active = false;
            course.IsPublish = false;

            _uof.courseWrite.UpdateCourse(course);
            await _uof.Commit();

            await _emailService.SendEmail(user.userName, user.email, "Are you sure you want to delete this course?",
                "you have 2 months for get it back otherwise will be removed and only people that already bought will have access");
        }
    }
}
