using Bogus;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services
{
    public static class SqidsBuild
    {
        public static SqidsEncoder<long> Build()
        {
            return new SqidsEncoder<long>(new SqidsOptions
            {
                Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
                MinLength = 4
            });
        }

        public static string GenerateRandomSqid()
        {
            var randomLong = new Faker().Random.Long(1, 2000);

            return Build().Encode(new[] { randomLong });
        }

        public static long GenerateRandomLong() => new Faker().Random.Long(1, 2000);
    }
}
