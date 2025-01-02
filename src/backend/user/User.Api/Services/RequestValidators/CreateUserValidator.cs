using FluentValidation;
using User.Api.DTOs;

namespace User.Api.Services.RequestValidators
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            //RuleFor(u => u.Email).NotEmpty().WithMessage(ResourceExceptMessages.EMAIL_NULL);
            //RuleFor(u => u.UserName).NotEmpty().WithMessage(ResourceExceptMessages.USERNAME_NULL);
            //RuleFor(u => u.Password).MinimumLength(8).WithMessage(ResourceExceptMessages.USERNAME_NULL);
            When(d => string.IsNullOrEmpty(d.Email), () =>
            {
                //RuleFor(d => d.Email).EmailAddress().WithMessage(ResourceExceptMessages.EMAIL_FORMAT);
            });
        }
    }
}
