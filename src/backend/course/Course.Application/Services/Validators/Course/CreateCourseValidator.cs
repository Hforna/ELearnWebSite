using Course.Communication.Requests;
using Course.Exception;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.Services.Validators.Course
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseRequest>
    {
        public CreateCourseValidator()
        {
            RuleFor(c => c.CourseLanguage).IsInEnum().WithMessage(ResourceExceptMessages.COURSE_LANGUAGE_OUT_ENUM);
            RuleFor(c => c.Description).MaximumLength(3000).WithMessage(ResourceExceptMessages.COURSE_DESCRIPTION_3000_LENGTH);
            RuleFor(c => c.Title).MaximumLength(256).WithMessage(ResourceExceptMessages.COURSE_TITLE_LENGTH_LESS_THAN_256);
        }
    }
}
