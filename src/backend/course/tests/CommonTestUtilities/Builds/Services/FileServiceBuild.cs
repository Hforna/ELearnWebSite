using Course.Application.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services
{
    public class FileServiceBuild
    {
        private static Mock<FileService> _mock = new Mock<FileService>();

        public static FileService Build()
        {
            return _mock.Object;
        }
    }
}
