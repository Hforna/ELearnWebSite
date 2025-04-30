using Course.Domain.Entitites.Quiz;
using Course.Domain.Repositories;
using Course.Infrastructure.Data.Course;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<AnswerOption>?> AnswerOptionsByQuestion(long questionId)
        {
            return await _dbContext.AnswerOptions.Where(d => d.QuestionId == questionId && d.Active).ToListAsync();
        }

        public async Task<List<QuestionEntity>?> QuestionsByQuiz(long quizId)
        {
            return await _dbContext.Questions.Where(d => d.QuizId == quizId && d.Active).ToListAsync();
        }

        public async Task<QuizEntity?> QuizById(long quizId)
        {
            return await _dbContext.Quizzes.Include(d => d.Questions).SingleOrDefaultAsync(d => d.Id == quizId && d.Active);
        }
    }
}
