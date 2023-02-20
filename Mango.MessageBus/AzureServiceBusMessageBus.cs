using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace Mango.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        private readonly string connectionString = @"Endpoint=sb://mangofoodrestaurant.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=kPFiSy7DsfJxcGDZXeKCxN+DistSk1vH+6GO4ydxPn0=";
        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topicName);
            string finaleMessage = JsonSerializer.Serialize<object>(message);
            ServiceBusMessage messsage = new(finaleMessage)
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(messsage);
        }
    }
}
