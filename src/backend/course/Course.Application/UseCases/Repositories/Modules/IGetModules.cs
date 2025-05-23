﻿using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IGetModules
    {
        public Task<ModulesResponse> Execute(long courseId);
    }
}
