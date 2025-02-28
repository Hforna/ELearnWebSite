﻿using Course.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Repositories.Executes
{
    public static class CourseWriteBuild
    {
        public static ICourseWriteOnly Build() => new Mock<ICourseWriteOnly>().Object;
    }
}
