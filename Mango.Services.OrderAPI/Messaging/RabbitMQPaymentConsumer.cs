using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models.AppSettings;
using Mango.Services.OrderAPI.Repository;
using MangoLibrary;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.OrderAPI.Messaging
{
    // it s an alterantive way.
    // we could implement the same interface of azureservice bus instead
    public class RabbitMPaymentConsumer : RabbitMQConsumerBackgroundService<RabbitMQConnectionSettings>
    {
        private readonly OrderRepository _orderRepository;
        private IModel _channel;

        public RabbitMPaymentConsumer(IOptions<RabbitMQConnectionSettings> rabbitMQConnectionSettings, OrderRepository orderRepository) : base(rabbitMQConnectionSettings)
        {
            _channel = Connection.CreateModel();
            _channel.ExchangeDeclare(ConnectionSettings.ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(ConnectionSettings.PaymentOrderUpdateQueue.QueueName, false, false, false);
            _channel.QueueBind(ConnectionSettings.PaymentOrderUpdateQueue.QueueName, ConnectionSettings.ExchangeName, ConnectionSettings.PaymentOrderUpdateQueue.RoutingKey);
            _orderRepository = orderRepository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var paymentResultMessage = JsonHelper.DeserializeIgnoringCase<UpdatePaymentResultMessage>(content);
                await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(ConnectionSettings.PaymentOrderUpdateQueue.QueueName, false, consumer);
            return Task.CompletedTask;
        }
    }
}
