using Course.Communication.Enums;
using Course.Communication.Requests;
using Course.Exception;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.Services.Validators.Review
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewRequest>
    {
        public CreateReviewValidator()
        {
            RuleFor(d => d.Rating).IsInEnum().WithMessage(ResourceExceptMessages.RATING_OUT_ENUM);
            RuleFor(d => d.Text).NotEmpty().WithMessage(ResourceExceptMessages.TEXT_CANT_BE_NULL);
            RuleFor(d => d.Text).MaximumLength(3000).WithMessage(ResourceExceptMessages.TEXT_LENGTH_MUST_BE_LESS_THAN_300);
        }
    }
}
