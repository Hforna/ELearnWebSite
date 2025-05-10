using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Repositories
{
    public interface IGenericRepository
    {
        public Task Add<T>(T entity);
        public void DeleteRange<T>(List<T> entities);
        public void Delete<T>(T entity);
    }
}
