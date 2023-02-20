using Azure.Messaging.ServiceBus;
using AzureMessageBus;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Models.AppSettings;
using Mango.Services.Email.Repository;
using MangoLibrary;
using Microsoft.Extensions.Options;
using System.Text;

namespace Mango.Services.Email.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly EmailRepository _emailRepository;
        private readonly AzureServiceBusSettings _azureServiceBusSettings;

        private ServiceBusProcessor _orderUpdatePaymentProcessor;

        public AzureServiceBusConsumer(EmailRepository emailRepository, IOptions<AzureServiceBusSettings> azureOptions)
        {
            _emailRepository = emailRepository;
            _azureServiceBusSettings = azureOptions.Value;
            var client = new ServiceBusClient(_azureServiceBusSettings.ConnectionString);
            _orderUpdatePaymentProcessor = client.CreateProcessor(_azureServiceBusSettings.OrderUpdatePaymentResultTopic, _azureServiceBusSettings.SubscriptionName);
        }
        public async Task Start()
        {
            _orderUpdatePaymentProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            _orderUpdatePaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await _orderUpdatePaymentProcessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _orderUpdatePaymentProcessor.StopProcessingAsync();
            await _orderUpdatePaymentProcessor.DisposeAsync();
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var body = Encoding.UTF8.GetString(args.Message.Body);

            UpdatePaymentResultMessage paymentResultMessage = JsonHelper.DeserializeIgnoringCase<UpdatePaymentResultMessage>(body);

            await _emailRepository.SendAndLogEmail(paymentResultMessage);
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
