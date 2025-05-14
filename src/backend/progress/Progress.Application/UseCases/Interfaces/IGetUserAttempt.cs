using AutoMapper;
using Progress.Application.Responses;
using Progress.Domain.Repositories;
using Progress.Domain.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.UseCases.Interfaces
{
    public interface IGetUserAttempt
    {
        public Task<QuizAttemptFullResponse> Execute(Guid attemptId, long courseId);
    }
}
