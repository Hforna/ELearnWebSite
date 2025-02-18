using Bogus;
using Course.Domain.Services.Azure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services
{
    public class StorageServiceBuild
    {
        public static IStorageService Build()
        {
            var storage = new Mock<IStorageService>();

            return storage.Object;
        }
    }
}
