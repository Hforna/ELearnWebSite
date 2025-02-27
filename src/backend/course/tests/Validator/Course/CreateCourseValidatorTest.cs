using CommonTestUtilities.Requests;
using FluentValidation;
using Course.Application.Services.Validators.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Course.Exception;
using Course.Communication.Enums;

namespace Validator.Course
{
    public class CreateCourseValidatorTest
    {
        [Fact]
        public void Sucess()
        {
            var request = new CreateCourseRequestTest().Build();
            var validator = new CreateCourseValidator();
            var result = validator.Validate(request);

            Assert.True(result.IsValid);
            Assert.False(result.Errors.Any());
        }

        [Fact]
        public void TitleOutOfLength()
        {
            var request = new CreateCourseRequestTest().Build();
            request.Title = new Faker().Lorem.Paragraphs(1000);
            var validator = new CreateCourseValidator();
            var result = validator.Validate(request);

            Assert.True(!result.IsValid);
            Assert.Contains(ResourceExceptMessages.COURSE_TITLE_LENGTH_LESS_THAN_256,
                result.Errors.Select(d => d.ErrorMessage).First());
        }

        [Fact]
        public void TypeCourseOutEnum()
        {
            var request = new CreateCourseRequestTest().Build();
            request.CourseLanguage = (LanguagesEnum) 20;
            var validator = new CreateCourseValidator();
            var result = validator.Validate(request);

            Assert.True(!result.IsValid);
            Assert.Contains(ResourceExceptMessages.COURSE_LANGUAGE_OUT_ENUM, 
                result.Errors.Select(d => d.ErrorMessage).First());
        }
    }
}
