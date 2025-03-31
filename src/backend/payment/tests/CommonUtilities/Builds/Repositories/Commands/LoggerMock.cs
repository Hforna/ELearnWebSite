using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Builds.Repositories.Commands
{
    public class LoggerMock<T>
    {
        public ILogger<T> Build() => new Mock<ILogger<T>>().Object;
    }
}
