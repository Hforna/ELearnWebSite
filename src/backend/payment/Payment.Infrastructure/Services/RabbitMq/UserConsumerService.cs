using MassTransit;
using Payment.Domain.Cons;
using Payment.Domain.Entities;
using Payment.Domain.Repositories;
using Payment.Domain.Services.Rest;
using SharedMessages.UserMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.RabbitMq
{
    public class UserConsumerService : IConsumer<UserCreatedMessage>, IConsumer<UserDeletedMessage>
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserRestService _userRest;

        public UserConsumerService(IUnitOfWork uof, IUserRestService userRest)
        {
            _uof = uof;
            _userRest = userRest;
        }

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

        public async Task Consume(ConsumeContext<UserDeletedMessage> context)
        {
            var userId = context.Message.UserId;
            if(context.Message.Teacher)
            {
                var balance = await _uof.balanceRead.BalanceByTeacherId(context.Message.UserId);

                if (balance is null)
                    throw new Exception("User balance doesn't exists");

                if (balance.BlockedBalance > 0 || balance.AvaliableBalance > 0)
                    throw new Exception("User can't delete account with a money amount in their balance");

                _uof.balanceWrite.Delete(balance);
            }
            var transaction = await _uof.transactionRead.TransactionsByUserId(userId);

            var payment = await _uof.paymentRead.PaymentByUser(userId);

            if (payment is not null)
                _uof.paymentWrite.Delete(payment);

            var payouts = await _uof.balanceRead.PayoutsByUser(userId);

            if (payouts is not null)
                _uof.balanceWrite.DeletePayoutRange(payouts);

            var orders = await _uof.orderRead.GetOrdersByUserId(userId);

            if(orders is not null)
                _uof.orderWrite.DeleteOrderRange(orders);

            await _uof.Commit();
        }
    }
}
