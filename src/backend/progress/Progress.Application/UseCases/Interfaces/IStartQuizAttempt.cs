﻿using Progress.Application.Responses;
using Progress.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.UseCases.Interfaces
{
    public interface IStartQuizAttempt
    {
        public Task<QuizAttemptResponse> Execute(long courseId, long quizId);
    }
}
