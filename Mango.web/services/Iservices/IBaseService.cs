using Mango.web.Models;

namespace Mango.web.services.Iservices
{
    public interface IBaseService:IDisposable
    {
        Task<T> SendAsync<T>(HttpRequestMessage message);
    }
}
