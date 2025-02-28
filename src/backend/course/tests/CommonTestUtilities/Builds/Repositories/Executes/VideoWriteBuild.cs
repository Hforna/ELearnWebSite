using Course.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Repositories.Executes
{
    public static class VideoWriteBuild
    {
        public static IVideoWriteOnly Build() => new Mock<IVideoWriteOnly>().Object;
    }
}
