using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.PaymentMessages
{
    public class UserGotRefound
    {
        public long UserId { get; set; }
        public List<long> CourseIds { get; set; }
    }
}
