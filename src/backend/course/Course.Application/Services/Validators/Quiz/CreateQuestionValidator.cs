using Course.Communication.Requests;
using Course.Exception;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.Services.Validators.Quiz
{
    public class CreateQuestionValidator : AbstractValidator<CreateQuestionRequest>
    {
        public CreateQuestionValidator()
        {
            RuleFor(d => d.QuestionText.Length).LessThanOrEqualTo(150).WithMessage(ResourceExceptMessages.QUESTION_TEXT_LESS_OR_EQUAL_150);
            RuleFor(d => d.Points).LessThanOrEqualTo(10).WithMessage(ResourceExceptMessages._10_MAX_POINTS);
            RuleFor(d => d.QuizType).IsInEnum().WithMessage(ResourceExceptMessages.QUIZ_TYPE_OUT_ENUM);
            When(d => d.QuizType == Communication.Enums.QuizTypeEnum.TRUE_FALSE, () =>
            {
                RuleFor(d => d.AnswerOptions.Count).Equal(2).WithMessage(ResourceExceptMessages.ANSWER_OPTION_JUST_TRUE_OR_FALSE);
                RuleForEach(d => d.AnswerOptions).Must(d => d.AnswerText.Equals("true") || d.AnswerText.Equals("false"));
            });
        }
    }
}
