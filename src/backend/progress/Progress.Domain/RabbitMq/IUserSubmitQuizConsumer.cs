﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.RabbitMq
{
    public interface IUserSubmitQuizConsumer
    {
        public Task Execute(string message);
    }
}
