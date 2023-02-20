using AzureMessageBus;
using Mango.Services.OrderAPI.Models.AppSettings;
using MangoLibrary;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Mango.Services.OrderAPI.Messaging.RabbitMQSender
{
    public class RabbitMQOrderMessageSender : IRabbitMQOrderMessageSender
    {
        private readonly RabbitMQConnectionSettings _rabbitMQSettings;
        private IConnection _connection;

        public RabbitMQOrderMessageSender(IOptions<RabbitMQConnectionSettings> rabbitMQSettings)
        {
            _rabbitMQSettings = rabbitMQSettings.Value;
        }
        public void Send(BaseMessage message, string queueName)
        {
            CreateConnection();
            using var channel = _connection.CreateModel();

            channel.QueueDeclare(queueName, false, false, false);
            var utf8message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<object>(message));
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: utf8message, mandatory: false);
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
