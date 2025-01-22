using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface ILessonReadOnly
    {
        public Task<Lesson?> LessonById(long id);
    }
}
