using AzureMessageBus;

namespace Mango.Services.OrderAPI.Messaging.RabbitMQSender
{
    public interface IRabbitMQOrderMessageSender
    {
        void Send(BaseMessage message, string queueName);
    }
}
