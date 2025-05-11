using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Dtos
{
    public class QuestionAnswerDto
    {
        public string QuestionId { get; set; }
        public string AnswerId { get; set; }
        public float points { get; set; }
    }
}
