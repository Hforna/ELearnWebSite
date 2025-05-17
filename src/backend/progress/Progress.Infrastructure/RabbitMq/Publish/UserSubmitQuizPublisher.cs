using Microsoft.Extensions.Configuration;
using Progress.Domain.Dtos;
using Progress.Domain.RabbitMq.Publisher;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Progress.Infrastructure.RabbitMq.Publish
{
    public class UserSubmitQuizPublisher : IUserSubmitQuizPublisher
    {
        private IChannel _channel;
        private IConfiguration _configuration;
        private IConnection _connection;

        public UserSubmitQuizPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMessage(UserSubmitDto message)
        {
            _connection = await new ConnectionFactory()
            {
                Port = _configuration.GetValue<int>("services:rabbitMq:port"),
                HostName = _configuration.GetValue<string>("services:rabbitMq:hostName")!,
                UserName = _configuration.GetValue<string>("services:rabbitMq:username"),
                Password = _configuration.GetValue<string>("services:rabbitMq:password"),
            }.CreateConnectionAsync();

            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("user_submit_exchange", "direct", true, false);

            var serialize = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(serialize);
            await _channel.BasicPublishAsync("user_submit_exchange", "user.submit", body);
        }
    }
}