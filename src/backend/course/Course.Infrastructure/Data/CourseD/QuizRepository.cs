﻿using Course.Domain.Entitites.Quiz;
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

        public async Task AddAnswerOptionsRange(List<AnswerOption> answerOptions)
        {
            await _dbContext.AnswerOptions.AddRangeAsync(answerOptions);
        }

        public async Task AddQuestion(QuestionEntity question)
        {
            await _dbContext.Questions.AddAsync(question);
        }

        public async Task<List<AnswerOption>?> AnswerOptionsByQuestion(long questionId)
        {
            return await _dbContext.AnswerOptions.Where(d => d.QuestionId == questionId && d.Active).ToListAsync();
        }

        public async Task<int> CountQuizzes(long courseId)
        {
            return await _dbContext.Quizzes.CountAsync(d => d.CourseId == courseId);
        }

        public void DeleteQuestion(QuestionEntity question)
        {
            _dbContext.Questions.Remove(question);
        }

        public void DeleteQuiz(QuizEntity quiz)
        {
            _dbContext.Quizzes.Remove(quiz);
        }

        public async Task<QuestionEntity?> QuestionByIdAndQuiz(long quizId, long questionId)
        {
            return await _dbContext.Questions.Include(d => d.Quiz).ThenInclude(d => d.Course).SingleOrDefaultAsync(d => d.Active && d.QuizId == quizId && d.Id == questionId);
        }

        public async Task<List<QuestionEntity>?> QuestionsByQuiz(long quizId)
        {
            return await _dbContext.Questions.Where(d => d.QuizId == quizId && d.Active).ToListAsync();
        }

        public async Task<QuizEntity?> QuizById(long quizId)
        {
            return await _dbContext.Quizzes.Include(d => d.Questions).Include(d => d.Course).SingleOrDefaultAsync(d => d.Id == quizId && d.Active);
        }
    }
}
