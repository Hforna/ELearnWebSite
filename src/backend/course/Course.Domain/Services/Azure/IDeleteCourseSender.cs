using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Services.Azure
{
    public interface IDeleteCourseSender
    {
        public Task SendMessage(long courseId);
    }
}
