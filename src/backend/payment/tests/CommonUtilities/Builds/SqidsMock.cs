using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
