using Course.Domain.Services.Azure;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services.Azure
{
    public static class NewModuleSenderBuild
    {
        public static INewModuleSender Build()
        {
            return new Mock<INewModuleSender>().Object;
        }
    }
}
