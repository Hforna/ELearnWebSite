using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Dtos.RabbitMqMessages
{
    public sealed record UserDeletedDto
    {
        public long userId { get; set; }
        public bool teacher { get; set; }
    }
}
