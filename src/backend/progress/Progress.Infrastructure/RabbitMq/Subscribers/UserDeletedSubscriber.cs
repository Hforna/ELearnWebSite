using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Progress.Domain.RabbitMq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Infrastructure.RabbitMq.Subscribers
{
    public class UserDeletedSubscriber : BackgroundService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IChannel _channel;
        private IConnection _connection;
        private readonly IUserDeletedConsumer _userDeletedConsumer;
        private readonly ILogger<UserDeletedSubscriber> _logger;

        public UserDeletedSubscriber(IConfiguration configuration, IUserDeletedConsumer userDeletedConsumer, ILogger<UserDeletedSubscriber> logger)
        {
            _configuration = configuration;
            _userDeletedConsumer = userDeletedConsumer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await new ConnectionFactory()
            {
                Port = _configuration.GetValue<int>("services:rabbitMq:port"),
                HostName = _configuration.GetValue<string>("services:rabbitMq:hostName")!,
                UserName = _configuration.GetValue<string>("services:rabbitMq:username"),
                Password = _configuration.GetValue<string>("services:rabbitMq:password"),
            }.CreateConnectionAsync();

            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("user_exchange", "direct", true);

            await _channel.QueueDeclareAsync("user-deleted", durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync("user-deleted", "user_exchange", "user.deleted");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (ModuleHandle, ea) =>
            {
                _logger.LogInformation("Processing message: {MessageId}", ea.BasicProperties.MessageId);

                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                await _userDeletedConsumer.Execute(message);
            };

            await _channel.BasicConsumeAsync("user-deleted", true, consumer);
        }

        public override void Dispose()
        {
            _channel.CloseAsync();
            _channel.Dispose();
            _connection.CloseAsync();
            _connection.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
