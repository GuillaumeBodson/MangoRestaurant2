using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace MangoLibrary.RabbitMQ
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
