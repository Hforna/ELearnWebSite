using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    public class BaseEntity
    {
        public long Id { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
