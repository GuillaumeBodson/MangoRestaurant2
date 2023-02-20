using Mango.Services.Email.Messages;
using Mango.Services.Email.Models.AppSettings;
using Mango.Services.Email.Repository;
using MangoLibrary;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.Email.Messaging.RabbitMQ
{
    // it s an alterantive way.
    // we could implement the same interface of azureservice bus instead
    public class RabbitMQPaymentConsumer : RabbitMQConsumerBackgroundService<RabbitMQConnectionSettings>
    {
        private readonly EmailRepository _emailRepository;
        private IModel _channel;

        public RabbitMQPaymentConsumer(IOptions<RabbitMQConnectionSettings> rabbitMQConnectionSettings, EmailRepository emailRepository) : base(rabbitMQConnectionSettings)
        {
            _emailRepository = emailRepository;
            _channel = Connection.CreateModel();
            _channel.ExchangeDeclare(ConnectionSettings.ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(ConnectionSettings.PaymentEamilUpdateQueue.QueueName, false, false, false);
            _channel.QueueBind(ConnectionSettings.PaymentEamilUpdateQueue.QueueName, ConnectionSettings.ExchangeName, ConnectionSettings.PaymentEamilUpdateQueue.RoutingKey);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var paymentRequestMessage = JsonHelper.DeserializeIgnoringCase<UpdatePaymentResultMessage>(content);
                await _emailRepository.SendAndLogEmail(paymentRequestMessage);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(ConnectionSettings.PaymentEamilUpdateQueue.QueueName, false, consumer);
            return Task.CompletedTask;
        }
    }
}
