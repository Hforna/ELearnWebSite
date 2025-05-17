using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Dtos
{
    public class UserSubmitDto
    {
        public long UserId { get; set; }
        public Guid AttemptId { get; set; }
        public long QuizId { get; set; }
    }
}
