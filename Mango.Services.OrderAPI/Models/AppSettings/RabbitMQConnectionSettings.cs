using Mango.Services.OrderAPI.Messaging;

namespace Mango.Services.OrderAPI.Models.AppSettings
{
    public class RabbitMQConnectionSettings : IRabbitMQConnectionSettings
    {
        public string HostName { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string QueueCheckoutMessageName { get; set; }
        public string QueueOrderPayementProcessName { get; set; }
        public string ExchangeName { get; set; }
        public QueueSettings PaymentOrderUpdateQueue { get; set; }
    }
    public class QueueSettings
    {
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
    }
}
