using AzureMessageBus;

namespace Mango.Services.ShoppingCartAPI.RabbitMQSender
{
    public interface IRabbitMQCartMessageSender
    {
        void Send(BaseMessage message, string queueName); 
    }
}
