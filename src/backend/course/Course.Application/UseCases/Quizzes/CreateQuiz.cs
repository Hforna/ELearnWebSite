using AutoMapper;
using Course.Application.UseCases.Repositories.Quizzes;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Entitites.Quiz;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Quizzes
{
    public class CreateQuiz : ICreateQuiz
    {
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public async Task<QuizResponse> Execute(CreateQuizRequest request)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            long? courseId = _sqids.Decode(request.CourseId).SingleOrDefault();

            if (courseId is null)
                throw new CourseException(ResourceExceptMessages.INVALID_ID_FORMAT, System.Net.HttpStatusCode.BadRequest);

            var course = await _uof.courseRead.CourseById((long)courseId);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            if (course.TeacherId != userId)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_OF_USER, System.Net.HttpStatusCode.Unauthorized);

            long? moduleId = _sqids.Decode(request.ModuleId).SingleOrDefault();

            if (moduleId is null)
                throw new ModuleException(ResourceExceptMessages.INVALID_ID_FORMAT, System.Net.HttpStatusCode.BadRequest);

            var module = await _uof.moduleRead.ModuleById((long)moduleId);

            if (module is null || module.CourseId != courseId)
                throw new ModuleException(ResourceExceptMessages.MODULE_DESCRIPTION_LENGTH, System.Net.HttpStatusCode.NotFound);

            var quiz = new QuizEntity()
            {
                CourseId = (long)courseId,
                ModuleId = (long)moduleId,
                Title = request.Title,
                PassingScore = request.PassingScore
            };

            await _uof.quizWrite.Add(quiz);
            await _uof.Commit();

            return _mapper.Map<QuizResponse>(quiz);
        }
    }
}
