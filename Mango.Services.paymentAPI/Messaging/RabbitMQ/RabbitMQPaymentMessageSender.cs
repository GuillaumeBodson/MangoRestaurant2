using AzureMessageBus;
using Mango.Services.paymentAPI.Models.AppSettings;
using MangoLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Mango.Services.paymentAPI.Messaging.RabbitMQ
{
    public class RabbitMQPaymentMessageSender : IRabbitMQPaymentMessageSender
    {
        private readonly RabbitMQConnectionSettings _rabbitMQSettings;
        private IConnection _connection;

        public RabbitMQPaymentMessageSender(IOptions<RabbitMQConnectionSettings> rabbitMQSettings)
        {
            _rabbitMQSettings = rabbitMQSettings.Value;
        }
        public void Send(BaseMessage message)
        {
            CreateConnection();
            using var channel = _connection.CreateModel();
            var utf8message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<object>(message));
            channel.ExchangeDeclare(_rabbitMQSettings.PayementUpdateExchange.ExchangeName, ExchangeType.Direct, false);

            foreach (var queue in _rabbitMQSettings.PayementUpdateExchange.Queues)
            {
                channel.QueueDeclare(queue.QueueName, false, false, false);
                channel.QueueBind(queue.QueueName, _rabbitMQSettings.PayementUpdateExchange.ExchangeName, queue.RoutingKey);
                channel.BasicPublish(exchange: _rabbitMQSettings.PayementUpdateExchange.ExchangeName, queue.RoutingKey, basicProperties: null, body: utf8message, mandatory: false);
            }
        }
        private void CreateConnection()
        {
            try
            {
                if (!ConnectionExist())
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = _rabbitMQSettings.HostName,
                        UserName = _rabbitMQSettings.Username,
                        Password = _rabbitMQSettings.Password,
                    };
                    _connection = factory.CreateConnection();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private bool ConnectionExist()
            => _connection != null;
    }
}
