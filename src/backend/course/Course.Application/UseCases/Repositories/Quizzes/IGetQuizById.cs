﻿using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Quizzes
{
    public interface IGetQuizById
    {
        public Task<QuizResponse> Execute(long courseId, long quizId, bool includeQuestions);
    }
}
