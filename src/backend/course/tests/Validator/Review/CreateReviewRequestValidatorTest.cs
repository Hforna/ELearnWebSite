using CommonTestUtilities.Requests;
using Course.Application.Services.Validators.Review;
using Course.Exception;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Review
{
    public class CreateReviewRequestValidatorTest
    {
        [Fact]
        public void Success()
        {
            var request = new CreateReviewRequestTest().Build(10);

            var validator = new CreateReviewValidator();
            var validate = validator.Validate(request);

            Assert.True(validate.IsValid);
        }

        [Fact]
        public void TextGreatherThan3000()
        {
            var request = new CreateReviewRequestTest().Build(500);

            var validator = new CreateReviewValidator();
            var validate = validator.Validate(request);

            validate.Errors.Should().Contain(e => e.ErrorMessage == ResourceExceptMessages.TEXT_LENGTH_MUST_BE_LESS_THAN_300);
        }
    }
}
