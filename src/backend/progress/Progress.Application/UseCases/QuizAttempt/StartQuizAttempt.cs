using AutoMapper;
using Progress.Application.Responses;
using Progress.Application.UseCases.Interfaces;
using Progress.Domain.Cache;
using Progress.Domain.Consts;
using Progress.Domain.Dtos;
using Progress.Domain.Entities;
using Progress.Domain.Exceptions;
using Progress.Domain.Repositories;
using Progress.Domain.Rest;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace Progress.Application.UseCases.QuizAttempt
{
    public class StartQuizAttempt : IStartQuizAttempt
    {
        private readonly IUserRestService _userRest;
        private readonly ICourseRestService _courseRest;
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IAttemptAnswerSession _attemptSession;
        private readonly IMapper _mapper;

        public StartQuizAttempt(IUserRestService userRest, ICourseRestService courseRest, IUnitOfWork uof, 
            SqidsEncoder<long> sqids, IAttemptAnswerSession attemptAnswerSession, IMapper mapper)
        {
            _userRest = userRest;
            _courseRest = courseRest;
            _uof = uof;
            _mapper = mapper;
            _attemptSession = attemptAnswerSession;
            _sqids = sqids;
        }

        public async Task<QuizAttemptResponse> Execute(long courseId, long quizId)
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var encodeCourseId = _sqids.Encode(courseId);
            var encodeQuizId = _sqids.Encode(quizId);

            var pendingQuiz = await _uof.quizAttemptRead.QuizAttemptPendingByUserId(userId);

            if (pendingQuiz is not null)
                throw new QuizException($"{ResourceExceptMessages.QUIZ_ATTEMPT_PENDING}: {pendingQuiz.Id}", System.Net.HttpStatusCode.Unauthorized);

            try
            {
                var userGotCourse = await _courseRest.UserGotCourse(encodeCourseId);

                if (!userGotCourse)
                    throw new CourseException(ResourceExceptMessages.USER_DOESNT_GOT_COURSE, System.Net.HttpStatusCode.Unauthorized);
            }
            catch(RestException re)
            {
                throw new RestException(re.GetMessage(), re.GetStatusCode());
            }

            var quiz = await _courseRest.GetQuiz(encodeQuizId, encodeCourseId);

            var quizAttempt = new QuizAttempts()
            {
                UserId = userId,
                Status = Domain.Enums.QuizAttemptStatusEnum.PEDNING,
                QuizId = quizId,
                CourseId = courseId
            };

            await _uof.genericRepository.Add<QuizAttempts>(quizAttempt);
            await _uof.Commit();

            var quizToSession = _mapper.Map<ShortQuizAnswersResponse>(quiz);

            _attemptSession.AddAnswers(quizToSession, quizAttempt.Id);

            return new QuizAttemptResponse()
            {
                ExpiresOn = DateTime.UtcNow.AddMinutes(QuizAttemtpsConsts.MinutesToExpire),
                QuizId = encodeQuizId,
                Quiz = quiz,
                StartedAt = quizAttempt.StartedAt,
                Id = quizAttempt.Id
            };
        }
    }
}
