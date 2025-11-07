using AutoMapper;
using Course.Application.Services.Validators.Quiz;
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
using System.Threading.Tasks;

namespace Course.Application.AppServices
{
    public interface IQuizService
    {
        Task<QuizResponse> CreateQuizAsync(CreateQuizRequest request);

        Task<QuestionResponse> CreateQuestionAsync(CreateQuestionRequest request, long quizId);

        Task<QuizResponse> GetQuizByIdAsync(long quizId, bool includeQuestions);

        Task DeleteQuizAsync(long quizId);

        Task DeleteQuestionAsync(long quizId, long questionId);
    }

    public class QuizService : IQuizService
    {
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _uof;

        public QuizService(
            IMapper mapper,
            SqidsEncoder<long> sqids,
            IUserService userService,
            IUnitOfWork uof)
        {
            _mapper = mapper;
            _sqids = sqids;
            _userService = userService;
            _uof = uof;
        }

        public async Task<QuizResponse> CreateQuizAsync(CreateQuizRequest request)
        {
            if (request.PassingScore > 10)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            long? courseId = _sqids.Decode(request.CourseId).SingleOrDefault();
            if (courseId is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var course = await _uof.courseRead.CourseById((long)courseId);
            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            if (course.TeacherId != userId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            long? moduleId = _sqids.Decode(request.ModuleId).SingleOrDefault();
            if (moduleId is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var module = await _uof.moduleRead.ModuleById((long)moduleId);
            if (module is null || module.CourseId != courseId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

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

        public async Task<QuestionResponse> CreateQuestionAsync(CreateQuestionRequest request, long quizId)
        {
            ValidateCreateQuestion(request);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var quiz = await _uof.quizRead.QuizById(quizId);
            if (quiz is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            if (quiz.Course.TeacherId != userId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

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

        public async Task<QuizResponse> GetQuizByIdAsync(long quizId, bool includeQuestions)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var quiz = await _uof.quizRead.QuizById(quizId);
            if (quiz is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var course = quiz.Course;

            var userGotCourse = await _uof.enrollmentRead.UserGotCourse(course.Id, userId);
            if (!userGotCourse)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var response = _mapper.Map<QuizResponse>(quiz);

            if (includeQuestions)
            {
                var questions = await _uof.quizRead.QuestionsByQuiz(quizId);
                var questionResponse = questions.OrderBy(d => Guid.NewGuid()).Select(async quest =>
                {
                    var questionResp = _mapper.Map<QuestionResponse>(quest);
                    var answerOptions = await _uof.quizRead.AnswerOptionsByQuestion(quest.Id);
                    questionResp.AnswerOptions = _mapper.Map<List<AnswerOptionsResponse>>(answerOptions);
                    return questionResp;
                });

                var taskQuestion = await Task.WhenAll(questionResponse);
                response.Questions = taskQuestion.ToList();
            }

            response.QuestionsNumber = quiz.Questions.Count;
            response.Metadata = new QuizMetadataResponse() { shuffledQuestions = true };

            return response;
        }

        public async Task DeleteQuizAsync(long quizId)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var quiz = await _uof.quizRead.QuizById(quizId);
            if (quiz is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var course = quiz.Course;
            if (course.TeacherId != userId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            _uof.quizWrite.DeleteQuiz(quiz);
            await _uof.Commit();
        }

        public async Task DeleteQuestionAsync(long quizId, long questionId)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var question = await _uof.quizRead.QuestionByIdAndQuiz(quizId, questionId);
            if (question is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            if (question.Quiz.Course.TeacherId != userId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            _uof.quizWrite.DeleteQuestion(question);
            await _uof.Commit();
        }

        private void ValidateCreateQuestion(CreateQuestionRequest request)
        {
            var validator = new CreateQuestionValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);
            }
        }
    }
}