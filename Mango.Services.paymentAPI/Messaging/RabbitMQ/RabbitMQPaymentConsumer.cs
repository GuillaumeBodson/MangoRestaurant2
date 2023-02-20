using AzureMessageBus;
using Mango.Services.OrderAPI.Messaging;
using Mango.Services.paymentAPI.Models;
using Mango.Services.paymentAPI.Models.AppSettings;
using MangoLibrary;
using Microsoft.Extensions.Options;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Mango.Services.paymentAPI.Messaging.RabbitMQ
{
    // it s an alterantive way.
    // we could implement the same interface of azureservice bus instead
    public class RabbitMQPaymentConsumer : RabbitMQConsumerBackgroundService<RabbitMQConnectionSettings>
    {
        private readonly IRabbitMQPaymentMessageSender _orderMessageSender;
        private readonly IProcessPayment _processPayment;
        private IModel _channel;

        public RabbitMQPaymentConsumer(IOptions<RabbitMQConnectionSettings> rabbitMQConnectionSettings, IRabbitMQPaymentMessageSender orderMessageSender, IProcessPayment processPayment) : base(rabbitMQConnectionSettings)
        {
            _orderMessageSender = orderMessageSender;
            _processPayment = processPayment;
            _channel = Connection.CreateModel();
            _channel.QueueDeclare(ConnectionSettings.QueueOrderPayementProcessName, false, false, false);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var paymentRequestMessage = JsonHelper.DeserializeIgnoringCase<PaymentRequestMessage>(content);
                await HandleMessage(paymentRequestMessage);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(ConnectionSettings.QueueOrderPayementProcessName, false, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentRequestMessage message)  
        {
            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                Status = result,
                OrderId = message.OrderId,
                Email = message.Email,
            };

            try
            {
                //await _messageBus.PublishMessage(updatePaymentResultMessage, _azureServiceBusSettings.OrderUpdatePaymentResultTopic);
                //await args.CompleteMessageAsync(args.Message);
                _orderMessageSender.Send(updatePaymentResultMessage);
            }
            catch
            {
                throw;
            }
        }
    }
}
