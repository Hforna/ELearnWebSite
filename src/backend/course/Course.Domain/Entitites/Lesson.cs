using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    public class Lesson : BaseEntity
    {
        [ForeignKey("Module")]
        public long ModuleId { get; set; }
        public Module Module { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public int TimeInMinutes { get; set; }
        public int Order { get; set; }
    }
}
