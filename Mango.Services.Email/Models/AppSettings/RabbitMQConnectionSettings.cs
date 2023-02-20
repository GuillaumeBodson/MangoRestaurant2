using Mango.Services.Email.Messaging.RabbitMQ;

namespace Mango.Services.Email.Models.AppSettings
{
    public class RabbitMQConnectionSettings : IRabbitMQConnectionSettings
    {
        public string HostName { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public QueueSettings PaymentEamilUpdateQueue { get; set; }
        public string ExchangeName { get; set; }
    }

    public class QueueSettings
    {
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
    }
}
