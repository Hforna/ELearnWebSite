using Bogus;
using CommonTestUtilities.Builds.Services;
using Course.Domain.DTOs;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.DTOs
{
    public class UserInfoDtoTest
    {
        public UserInfosDto Build()
        {
            return new Faker<UserInfosDto>()
                .RuleFor(d => d.id, f => SqidsBuild.Build().Encode(new[] { f.Random.Long(1, 20000) }))
                .RuleFor(d => d.email, f => f.Internet.Email())
                .RuleFor(d => d.userName, f => f.Person.FirstName)
                .RuleFor(d => d.phoneNumber, f => f.Person.Phone)
                .RuleFor(d => d.is2fa, f => f.PickRandom<bool>(true, false));
        }
    }
}
