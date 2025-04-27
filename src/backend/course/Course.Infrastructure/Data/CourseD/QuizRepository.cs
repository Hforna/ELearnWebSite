using Course.Domain.Entitites.Quiz;
using Course.Domain.Repositories;
using Course.Infrastructure.Data.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data.CourseD
{
    public class QuizRepository : IQuizReadOnly, IQuizWriteOnly
    {
        private readonly CourseDbContext _dbContext;

        public QuizRepository(CourseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(QuizEntity quiz)
        {
            await _dbContext.Quizzes.AddAsync(quiz);
        }
    }
}
