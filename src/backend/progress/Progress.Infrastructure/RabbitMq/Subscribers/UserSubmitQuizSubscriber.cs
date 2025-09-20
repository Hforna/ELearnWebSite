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

namespace Progress.Infrastructure.RabbitMq.Subscribers
{
    public class UserSubmitQuizSubscriber : BackgroundService, IDisposable
    {
        private IConfiguration _configuration;
        private IChannel _channel;
        private IConnection _connection;
        private readonly IUserSubmitQuizConsumer _userSubmitConsumer;

        public UserSubmitQuizSubscriber(IConfiguration configuration, IUserSubmitQuizConsumer userSubmitQuiz)
        {
            _configuration = configuration;
            _userSubmitConsumer = userSubmitQuiz;
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

            await _channel.ExchangeDeclareAsync("user_submit_exchange", "direct", true);

            await _channel.QueueDeclareAsync("user-submit", true);
            await _channel.QueueBindAsync("user-submit", "user_submit_exchange", "user.submit");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (ModuleHandle, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                await _userSubmitConsumer.Execute(message);
            };
            await _channel.BasicConsumeAsync("user-submit", false, consumer);
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
