using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.UseCases.Interfaces.Progress
{
    public interface ICompleteLesson
    {
        public Task Execute(long lessonId);
    }
}
