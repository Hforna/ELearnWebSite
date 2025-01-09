using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    public class Lesson : BaseEntitie
    {
        [ForeignKey("Module")]
        public long ModuleId { get; set; }
        public Module Module { get; set; }
        public string Title { get; set; }
    }
}
