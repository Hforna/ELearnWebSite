using Course.Communication.Requests;
using Course.Exception;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.Services.Validators.Module
{
    public class UpdateModuleValidator : AbstractValidator<UpdateModuleRequest>
    {
        public UpdateModuleValidator()
        {
            RuleFor(d => d.Name).MaximumLength(100).WithMessage(ResourceExceptMessages.MODULE_NAME_LENGTH);
            RuleFor(d => d.Name).MaximumLength(100).WithMessage(ResourceExceptMessages.MODULE_DESCRIPTION_LENGTH);
            RuleFor(d => d.Position).GreaterThan(0).WithMessage(ResourceExceptMessages.MODULE_POSITION_GREATHER_OR_EQUAL_ZERO);
        }
    }
}
