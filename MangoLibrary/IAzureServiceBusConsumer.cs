using System.Threading.Tasks;

namespace MangoLibrary
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}