using AzureMessageBus;

namespace Mango.Services.paymentAPI.Messaging.RabbitMQ
{
    public interface IRabbitMQPaymentMessageSender
    {
        void Send(BaseMessage message);
    }
}
