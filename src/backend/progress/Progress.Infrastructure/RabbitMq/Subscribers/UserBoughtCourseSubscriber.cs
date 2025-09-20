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
    public class UserBoughtCourseSubscriber : BackgroundService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IChannel _channel;
        private readonly IUserBoughtCourseConsumer _userBoughtCourse;

        public UserBoughtCourseSubscriber(IConfiguration configuration, IConnection connection,
            IChannel channel, IUserBoughtCourseConsumer userBoughtCourse)
        {
            _configuration = configuration;
            _connection = connection;
            _channel = channel;
            _userBoughtCourse = userBoughtCourse;
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

            await _channel.ExchangeDeclareAsync("payment_exchange", "direct");

            await _channel.QueueDeclareAsync("allow-course", true, false, false);
            await _channel.QueueBindAsync("allow-course", "payment_exchange", "allow.course");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (ModuleHandle, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                try
                {
                    await _userBoughtCourse.Execute(message);

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };
            await _channel.BasicConsumeAsync("allow-course", false, consumer);

            await Task.CompletedTask;
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
