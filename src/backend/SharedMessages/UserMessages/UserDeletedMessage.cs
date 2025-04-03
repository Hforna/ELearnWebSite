using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.UserMessages
{
    public class UserDeletedMessage
    {
        public long UserId { get; set; }
        public bool Teacher { get; set; }
    }
}
