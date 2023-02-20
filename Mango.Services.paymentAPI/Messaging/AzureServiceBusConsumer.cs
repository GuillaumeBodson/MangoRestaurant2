using Azure.Messaging.ServiceBus;
using AzureMessageBus;
using Mango.Services.paymentAPI.Models;
using Mango.Services.paymentAPI.Models.AppSettings;
using MangoLibrary;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using PaymentProcessor;
using System.Text;

namespace Mango.Services.paymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IMessageBus _messageBus;
        private readonly IProcessPayment _processPayment;
        private readonly AzureServiceBusSettings _azureServiceBusSettings;

        private ServiceBusProcessor _orderPaymentProcessor;

        public AzureServiceBusConsumer(IOptions<AzureServiceBusSettings> azureOptions, IMessageBus messageBus, IProcessPayment processPayment)
        {
            _messageBus = messageBus;
            _processPayment = processPayment;
            _azureServiceBusSettings = azureOptions.Value;
            var client = new ServiceBusClient(_azureServiceBusSettings.ConnectionString);
            _orderPaymentProcessor = client.CreateProcessor(_azureServiceBusSettings.OrderPaymentProcessTopic, _azureServiceBusSettings.OrderPaymentProcessSubscription);
        }
        public async Task Start()
        {
            _orderPaymentProcessor.ProcessMessageAsync += ProcessPayment;
            _orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await _orderPaymentProcessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _orderPaymentProcessor.StopProcessingAsync();
            await _orderPaymentProcessor.DisposeAsync();
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task ProcessPayment(ProcessMessageEventArgs args)
        {
            var body = Encoding.UTF8.GetString(args.Message.Body);

            PaymentRequestMessage paymentRequestMessage = JsonHelper.DeserializeIgnoringCase<PaymentRequestMessage>(body);

            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId,
                Email = paymentRequestMessage.Email,
            };

            try
            {
                await _messageBus.PublishMessage(updatePaymentResultMessage, _azureServiceBusSettings.OrderUpdatePaymentResultTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch
            {
                throw;
            }
        }
    }
}
