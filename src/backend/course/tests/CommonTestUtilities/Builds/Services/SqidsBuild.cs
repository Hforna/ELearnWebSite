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
    }
}
