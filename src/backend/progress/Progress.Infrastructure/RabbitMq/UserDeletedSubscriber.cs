using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Progress.Domain.RabbitMq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Infrastructure.RabbitMq
{
    public class UserDeletedSubscriber : BackgroundService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IChannel _channel;
        private IConnection _connection;
        private readonly IUserDeletedConsumer _userDeletedConsumer;
        
        public UserDeletedSubscriber(IConfiguration configuration, IUserDeletedConsumer userDeletedConsumer)
        {
            _configuration = configuration;
            _userDeletedConsumer = userDeletedConsumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await new ConnectionFactory()
            {
                Port = int.Parse(_configuration.GetConnectionString("rabbitMq:port")!),
                HostName = _configuration.GetConnectionString("rabbitMq")!
            }.CreateConnectionAsync();

            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("user_exchange", "direct", true);
            await _channel.QueueBindAsync("user-deleted", "user_exchange", "user.deleted");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (ModuleHandle, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                await _userDeletedConsumer.Execute(message);
            };

            await _channel.BasicConsumeAsync("user-deleted", true, consumer);

            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.CloseAsync();
            _connection.DisposeAsync();
            base.Dispose();
        }
    }
}
