using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Entities
{
    [Table("certificates")]
    public class Certificate
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public DateTime? IssueDate { get; set; }
        public string PdfUrl { get; set; }
        public string ValidationHash { get; set; }
    }
}
