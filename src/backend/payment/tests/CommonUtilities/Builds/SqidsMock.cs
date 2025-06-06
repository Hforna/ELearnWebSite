﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Moq;
using Sqids;

namespace CommonUtilities.Builds
{
    public static class SqidsMock
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
    }
}
