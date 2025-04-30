using AutoMapper;
using Course.Application.UseCases.Repositories.Quizzes;
using Course.Communication.Responses;
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
    public class GetQuizById : IGetQuizById
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;

        public GetQuizById(IMapper mapper, IUserService userService, 
            IUnitOfWork uof, SqidsEncoder<long> sqids)
        {
            _mapper = mapper;
            _userService = userService;
            _uof = uof;
            _sqids = sqids;
        }

        public async Task<QuizResponse> Execute(long courseId, long quizId, bool includeQuestions)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var userGotCourse = await _uof.enrollmentRead.UserGotCourse(courseId, userId);

            if (!userGotCourse)
                throw new CourseException(ResourceExceptMessages.USER_DOESNT_GOT_COURSE, System.Net.HttpStatusCode.Unauthorized);

            var quiz = await _uof.quizRead.QuizById(quizId);

            if (quiz is null)
                throw new QuizException(ResourceExceptMessages.QUIZ_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var response = _mapper.Map<QuizResponse>(quiz);

            if(includeQuestions)
            {
                var questions = await _uof.quizRead.QuestionsByQuiz(quizId);
                var questionResponse = questions.OrderBy(d => Guid.NewGuid()).Select(async quest =>
                {
                    var response = _mapper.Map<QuestionResponse>(quest);

                    var answerOptions = await _uof.quizRead.AnswerOptionsByQuestion(quest.Id);
                    response.AnswerOptions = _mapper.Map<List<AnswerOptionsResponse>>(answerOptions);

                    return response;
                });
                var taskQuestion = await Task.WhenAll(questionResponse);

                response.Questions = taskQuestion.ToList();
            }

            response.QuestionsNumber = quiz.Questions.Count;
            response.Metadata = new QuizMetadataResponse() { shuffledQuestions = true };

            return response;
        }
    }
}
