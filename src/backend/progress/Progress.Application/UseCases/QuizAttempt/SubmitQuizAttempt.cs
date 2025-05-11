using AutoMapper;
using Progress.Application.Requests;
using Progress.Application.Responses;
using Progress.Application.UseCases.Interfaces;
using Progress.Domain.Cache;
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

namespace Progress.Application.UseCases.QuizAttempt
{
    public class SubmitQuizAttempt : ISubmitQuizAttempt
    {
        private readonly IUserRestService _userRest;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IMapper _mapper;
        private readonly IAttemptAnswerSession _attemptAnswer;
        private readonly IUnitOfWork _uof;

        public async Task<QuizSubmitResponse> Execute(SubmitQuizRequest request)
        {
            var quizAnswerResponse = _attemptAnswer.GetAttemptQuestionAnswers(request.AttemptId);
            var correctAnswers = quizAnswerResponse.AnswerDtos.OrderBy(d => d.QuestionId).ToList();
            var userAnswers = request.Questions.OrderBy(d => d.QuestionId).ToList();

            if (request.Questions is null || request.Questions.Count != correctAnswers.Count())
                throw new QuizAttemptException(ResourceExceptMessages.USER_DOESNT_ANSWERED_ALL_QUESTIONS, System.Net.HttpStatusCode.BadRequest);

            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var attempt = await _uof.quizAttemptRead.QuizAttemptByUserAndId(userId, request.AttemptId);

            if (attempt is null)
                throw new QuizAttemptException(ResourceExceptMessages.QUIZ_ATTEMPT_NOT_EXISTS, System.Net.HttpStatusCode.NotFound);

            float userPoints = 0;

            var questionAnswersResponse = new List<QuestionAnswerResponse>();

            for(var i = 0; i < userAnswers.Count(); i++)
            {
                var answer = userAnswers[i].AnswerId;
                var question = _sqids.Decode(userAnswers[i].QuestionId).Single();

                var questionResponse = new QuestionAnswerResponse() { QuestionId = userAnswers[i].QuestionId };
                var userResponse = new UserQuizResponse()
                {
                    AttemptId = attempt.Id,
                    QuestionId = question,
                    SelectedOption = _sqids.Decode(userAnswers[i].AnswerId).Single(),
                };

                if (answer == correctAnswers[i].AnswerId)
                {
                    questionResponse.isCorrect = true;
                    userResponse.IsCorrect = true;

                    userPoints += correctAnswers[i].points;
                } else
                {
                    questionResponse.isCorrect = false;
                    userResponse.IsCorrect = false;
                }
                attempt.QuizResponses.Add(userResponse);
                questionAnswersResponse.Add(questionResponse);
            }

            attempt.Score = (decimal)userPoints;
            attempt.Status = Domain.Enums.QuizAttemptStatusEnum.FINISHED;
            attempt.AttemptedAt = DateTime.UtcNow;
            attempt.Passed = (decimal)userPoints >= quizAnswerResponse.PassingScore;

            _uof.genericRepository.Update<QuizAttempts>(attempt);
            await _uof.Commit();

            return new QuizSubmitResponse()
            {
                AttemptId = request.AttemptId,
                QuizId = _sqids.Encode(attempt.QuizId),
                TotalPoints = userPoints,
                Passed = attempt.Passed
            };
        }
    }
}
