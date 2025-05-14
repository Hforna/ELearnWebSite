using AutoMapper;
using Progress.Application.Responses;
using Progress.Application.UseCases.Interfaces;
using Progress.Domain.Cache;
using Progress.Domain.Exceptions;
using Progress.Domain.Repositories;
using Progress.Domain.Rest;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.UseCases.QuizAttempt
{
    public class GetUserAttempt : IGetUserAttempt
    {
        private readonly IUserRestService _userRest;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;
        private readonly ICourseRestService _courseRest;

        public GetUserAttempt(IUserRestService userRest, IMapper mapper, IUnitOfWork uof, SqidsEncoder<long> sqids, ICourseRestService courseRest)
        {
            _courseRest = courseRest;
            _userRest = userRest;
            _uof = uof;
            _sqids = sqids;
            _mapper = mapper;
        }

        public async Task<QuizAttemptFullResponse> Execute(Guid attemptId, long courseId)
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var attempt = await _uof.quizAttemptRead.QuizAttemptByUserAndId(userId, attemptId);

            if (attempt is null)
                throw new QuizAttemptException(ResourceExceptMessages.QUIZ_ATTEMPT_NOT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var quiz = await _courseRest.GetQuiz(_sqids.Encode(attempt.QuizId), _sqids.Encode(courseId));

            if (quiz is null)
                throw new QuizException(ResourceExceptMessages.QUIZ_ATTEMPT_NOT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var response = _mapper.Map<QuizAttemptFullResponse>(attempt);
            response.QuestionAnswers = _mapper.Map<List<QuestionAnswerResponse>>(quiz.Questions);

            return response;
        }
    }
}
