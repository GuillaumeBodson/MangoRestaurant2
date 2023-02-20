using Azure.Messaging.ServiceBus;
using AzureMessageBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.AppSettings;
using Mango.Services.OrderAPI.Repository;
using MangoLibrary;
using Microsoft.Extensions.Options;
using System.Text;

namespace Mango.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly OrderRepository _orderRepository;
        private readonly IMessageBus _messageBus;
        private readonly AzureServiceBusSettings _azureServiceBusSettings;

        private ServiceBusProcessor _checkoutProcessor;
        private ServiceBusProcessor _orderUpdatePaymentProcessor;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IOptions<AzureServiceBusSettings> azureOptions, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _messageBus = messageBus;
            _azureServiceBusSettings = azureOptions.Value;
            var client = new ServiceBusClient(_azureServiceBusSettings.ConnectionString);
            _checkoutProcessor = client.CreateProcessor(_azureServiceBusSettings.CheckoutMessageTopic, _azureServiceBusSettings.OrderSubscriptionName);
            _orderUpdatePaymentProcessor = client.CreateProcessor(_azureServiceBusSettings.OrderUpdatePaymentResultTopic, _azureServiceBusSettings.OrderSubscriptionName);
        }
        public async Task Start()
        {
            _checkoutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            _checkoutProcessor.ProcessErrorAsync += ErrorHandler;
            await _checkoutProcessor.StartProcessingAsync();

            _orderUpdatePaymentProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            _orderUpdatePaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await _orderUpdatePaymentProcessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _checkoutProcessor.StopProcessingAsync();
            await _checkoutProcessor.DisposeAsync();

            await _orderUpdatePaymentProcessor.StopProcessingAsync();
            await _orderUpdatePaymentProcessor.DisposeAsync();
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
        {
            var body = Encoding.UTF8.GetString(args.Message.Body);

            CheckoutHeaderDto checkoutHeader = JsonHelper.DeserializeIgnoringCase<CheckoutHeaderDto>(body);

            //TODO : add mapping profile
            OrderHeader orderHeader = new()
            {
                CartNumber = checkoutHeader.CartNumber,
                CartTotalItems = checkoutHeader.CartTotalItems,
                CouponCode = checkoutHeader.CouponCode,
                CVV = checkoutHeader.CVV,
                DiscountTotal = checkoutHeader.DiscountTotal,
                Email = checkoutHeader.Email,
                ExpiryMonthYear = checkoutHeader.ExpiryMonthYear,
                FirstName = checkoutHeader.FirstName,
                LastName = checkoutHeader.LastName,
                PickupDate = checkoutHeader.PickupDate,
                Phone = checkoutHeader.Phone,
                UserId = checkoutHeader.UserId,
                OrderDetails = checkoutHeader.CartDetails.Select(x => new OrderDetails()
                {
                    Count = x.Count,
                    ProductId = x.ProductId,
                    ProductPrice = x.Product.Price,
                    ProductName = x.Product.Name,
                }).ToList(),
                OrderTime = DateTime.Now,
                OrderTotal = checkoutHeader.OrderTotal,
                PaymenStatus = false
            };
            orderHeader.CartTotalItems = orderHeader.OrderDetails.Sum(x => x.Count);

            await _orderRepository.AddOrder(orderHeader);

            PaymentRequestMessage paymentRequestMessage = new()
            {
                CardNumber = orderHeader.CartNumber,
                CVV = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderId = orderHeader.OrderHeaderId,
                OrderTotal = orderHeader.OrderTotal,
                Name = orderHeader.FirstName + " " + orderHeader.LastName,
                Email = orderHeader.Email,
            };

            try
            {
                await _messageBus.PublishMessage(paymentRequestMessage, _azureServiceBusSettings.OrderPaymentProcessTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch
            {
                throw;
            }
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var body = Encoding.UTF8.GetString(args.Message.Body);

            UpdatePaymentResultMessage paymentResultMessage = JsonHelper.DeserializeIgnoringCase<UpdatePaymentResultMessage>(body);

            await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
