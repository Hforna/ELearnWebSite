using AutoMapper;
using Course.Application.Services.Validators.Quiz;
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
    public class CreateQuestion : ICreateQuestion
    {
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _uof;

        public CreateQuestion(IMapper mapper, SqidsEncoder<long> sqids, 
            IUserService userService, IUnitOfWork uof)
        {
            _mapper = mapper;
            _sqids = sqids;
            _userService = userService;
            _uof = uof;
        }

        public async Task<QuestionResponse> Execute(CreateQuestionRequest request)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var quizId = _sqids.Decode(request.QuizId).Single();
            var quiz = await _uof.quizRead.QuizById(quizId);

            if (quiz is null)
                throw new QuizException(ResourceExceptMessages.QUIZ_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            if (quiz.Course.TeacherId != userId)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST, System.Net.HttpStatusCode.Unauthorized);

            var question = _mapper.Map<QuestionEntity>(request);

            var answerOptions = _mapper.Map<List<AnswerOption>>(request.AnswerOptions);

            await _uof.quizWrite.AddQuestion(question);
            await _uof.quizWrite.AddAnswerOptionsRange(answerOptions.Select(ans =>
            {
                ans.QuestionId = question.Id;

                return ans;
            }).ToList());

            await _uof.Commit();

            var response = _mapper.Map<QuestionResponse>(question);
            response.AnswerOptions = _mapper.Map<List<AnswerOptionsResponse>>(answerOptions);

            return response;
        }

        void Validate(CreateQuestionRequest request)
        {
            var validator = new CreateQuestionValidator();
            var result = validator.Validate(request);

            if(!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new QuizException(errorMessages, System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}
