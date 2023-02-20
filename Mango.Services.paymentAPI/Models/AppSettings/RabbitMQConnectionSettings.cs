using Mango.Services.OrderAPI.Messaging;
using Mango.Services.paymentAPI.Messaging.RabbitMQ;

namespace Mango.Services.paymentAPI.Models.AppSettings
{
    public class RabbitMQConnectionSettings : IRabbitMQConnectionSettings
    {
        public string HostName { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string QueueOrderPayementProcessName { get; set; }
        public string QueueUpdatePaymentResultName { get; set; }
        //public string ExchangeName { get; set; }
        //public string PaymentEamilUpdateQueueName { get; set; }
        //public string PaymentOrderUpdateQueueName { get; set; }
        public DirectExchangeSettings PayementUpdateExchange { get; set; }
    }

    public class DirectExchangeSettings
    {
        public string ExchangeName { get; set; }

        public List<QueueSettings> Queues { get; set; }
    }
    public class QueueSettings
    {
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
    }
}
