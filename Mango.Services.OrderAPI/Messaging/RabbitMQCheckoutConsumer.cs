using AzureMessageBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Messaging.RabbitMQSender;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.AppSettings;
using Mango.Services.OrderAPI.Repository;
using MangoLibrary;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Mango.Services.OrderAPI.Messaging
{
    // it s an alterantive way.
    // we could implement the same interface of azureservice bus instead
    public class RabbitMQCheckoutConsumer : RabbitMQConsumerBackgroundService<RabbitMQConnectionSettings>
    {
        private readonly OrderRepository _orderRepository;
        private readonly IRabbitMQOrderMessageSender _orderMessageSender;
        private IModel _channel;

        public RabbitMQCheckoutConsumer(OrderRepository orderRepository, IOptions<RabbitMQConnectionSettings> rabbitMQConnectionSettings, IRabbitMQOrderMessageSender orderMessageSender) : base(rabbitMQConnectionSettings)
        {
            _orderRepository = orderRepository;
            _orderMessageSender = orderMessageSender;
            _channel = Connection.CreateModel();
            _channel.QueueDeclare(ConnectionSettings.QueueCheckoutMessageName, false, false, false);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var checkoutHeaderDto = JsonHelper.DeserializeIgnoringCase<CheckoutHeaderDto>(content);
                await HandleMessage(checkoutHeaderDto);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(ConnectionSettings.QueueCheckoutMessageName, false, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(CheckoutHeaderDto checkoutHeader)
        {
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
                //await _messageBus.PublishMessage(paymentRequestMessage, _azureServiceBusSettings.OrderPaymentProcessTopic);
                //await args.CompleteMessageAsync(args.Message);
                _orderMessageSender.Send(paymentRequestMessage, ConnectionSettings.QueueOrderPayementProcessName);
            }
            catch
            {
                throw;
            }
        }
    }
}
