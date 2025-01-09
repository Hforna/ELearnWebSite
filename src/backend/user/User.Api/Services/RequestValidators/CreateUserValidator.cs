using FluentValidation;
using User.Api.DTOs;

namespace User.Api.Services.RequestValidators
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email cannot be null");
            RuleFor(u => u.UserName).NotEmpty().WithMessage("Usernae can't be null");
            RuleFor(u => u.Password).MinimumLength(8).WithMessage("Password length must have 8 digits or more");
            When(d => string.IsNullOrEmpty(d.Email), () =>
            {
                RuleFor(d => d.Email).EmailAddress().WithMessage("Email format wrong");
            });
        }
    }
}
