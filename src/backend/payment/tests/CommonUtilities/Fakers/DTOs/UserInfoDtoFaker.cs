using Bogus;
using Bogus.Extensions.Sweden;
using CommonUtilities.Builds;
using Payment.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Fakers.DTOs
{
    public class UserInfoDtoFaker
    {
        public UserInfoDto Build()
        {
            var id = SqidsMock.Build().Encode(1);

            return new Faker<UserInfoDto>()
                .RuleFor(d => d.is2fa, true)
                .RuleFor(d => d.email, f => f.Person.Email)
                .RuleFor(d => d.id, id)
                .RuleFor(d => d.userName, f => f.Person.UserName)
                .RuleFor(d => d.phoneNumber, f => f.Person.Phone);
        }
    }
}
