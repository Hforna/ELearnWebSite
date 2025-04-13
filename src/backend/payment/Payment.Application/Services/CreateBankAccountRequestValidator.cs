using FluentValidation;
using Payment.Application.Requests;
using Payment.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services
{
    public class CreateBankAccountRequestValidator : AbstractValidator<CreateBankAccountRequest>
    {
        public CreateBankAccountRequestValidator()
        {
            RuleFor(d => d.Email).EmailAddress().WithMessage(ResourceExceptMessages.EMAIL_FORMAT_INVALID);
            RuleFor(d => d.FirstName.Length).LessThanOrEqualTo(30).WithMessage(ResourceExceptMessages.INVALID_FIRST_NAME);
            RuleFor(d => d.FirstName.Length).LessThanOrEqualTo(60).WithMessage(ResourceExceptMessages.LAST_NAME_INVALID);
        }
    }
}
