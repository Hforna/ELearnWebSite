using Progress.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Infrastructure.Data
{
    public class GenericRepository : IGenericRepository
    {
        private readonly ProjectDbContext _dbContext;

        public GenericRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add<T>(T entities)
        {
            await _dbContext.AddAsync(entities);
        }

        public void Delete<T>(T entity)
        {
            _dbContext.Remove(entity);
        }

        public void DeleteRange<T>(List<T> entities)
        {
            _dbContext.RemoveRange(entities);
        }

        public void Update<T>(T entity)
        {
            _dbContext.Update(entity);
        }
    }
}
