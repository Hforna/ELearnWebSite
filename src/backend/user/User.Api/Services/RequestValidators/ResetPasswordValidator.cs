using FluentValidation;
using User.Api.DTOs;

namespace User.Api.Services.RequestValidators
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Password).MinimumLength(8).WithMessage("Password length must be 8 or more digits");
            RuleFor(d => d.NewPassword).Matches(d => d.Password).WithMessage("Password must be equals");
        }
    }
}
