﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Course
{
    public interface IDeleteCourse
    {
        public Task Execute(long id);
    }
}