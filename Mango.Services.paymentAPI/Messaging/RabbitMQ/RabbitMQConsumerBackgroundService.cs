using Mango.Services.paymentAPI.Messaging.RabbitMQ;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Mango.Services.OrderAPI.Messaging
{
    public abstract class RabbitMQConsumerBackgroundService<TSettings> : BackgroundService where TSettings : class, IRabbitMQConnectionSettings
    {
        protected IConnection Connection { get; private set; }
        protected TSettings ConnectionSettings { get; }

        public RabbitMQConsumerBackgroundService(IOptions<TSettings> connectionSettings)
        {
            ConnectionSettings = connectionSettings.Value;
            CreateConnection();
        }
        protected void CreateConnection()
        {
            try
            {
                if (!ConnectionExist())
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = ConnectionSettings.HostName,
                        UserName = ConnectionSettings.Username,
                        Password = ConnectionSettings.Password,
                    };
                    Connection = factory.CreateConnection();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private bool ConnectionExist()
            => Connection != null;
    }
}
