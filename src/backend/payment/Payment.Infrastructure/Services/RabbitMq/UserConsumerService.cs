using MassTransit;
using Payment.Domain.Cons;
using Payment.Domain.Entities;
using Payment.Domain.Repositories;
using SharedMessages.UserMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.RabbitMq
{
    public class UserConsumerService : IConsumer<UserCreatedMessage>
    {
        private readonly IUnitOfWork _uof;

        public async Task Consume(ConsumeContext<UserCreatedMessage> context)
        {
            if(context.Message.Teacher)
            {
                var balance = new Balance()
                {
                    AvaliableBalance = 0,
                    BlockedBalance = 0,
                    TeacherId = context.Message.UserId
                };

                await _uof.balanceWrite.Add(balance);
                await _uof.Commit();
            }
        }
    }
}
