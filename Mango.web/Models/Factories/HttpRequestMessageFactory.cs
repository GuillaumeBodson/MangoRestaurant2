
using MangoLibrary;

namespace Mango.web.Models.Factories
{
    public interface IHttpRequestMessageFactory
    {
        HttpRequestMessage CreateDelete(string url, object data = null);
        HttpRequestMessage CreateGet(string url);
        HttpRequestMessage CreatePost(string url, object data);
        HttpRequestMessage CreatePut(string url, object data);
    }

    public class HttpRequestMessageFactory : IHttpRequestMessageFactory
    {
        private readonly Func<HttpRequestMessage> _factoryMethod;

        public HttpRequestMessageFactory(Func<HttpRequestMessage> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        private HttpRequestMessage Create(HttpMethod apiType, string url, object data = null)
        {
            var requestMessage = _factoryMethod();

            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.RequestUri = new(url);

            requestMessage.Content = data.ToUTF8EncodedJsonStringContent();

            requestMessage.Method = apiType;

            return requestMessage;
        }
        public HttpRequestMessage CreateGet(string url)
            => Create(HttpMethod.Get, url);

        public HttpRequestMessage CreatePost(string url, object data)
            => Create(HttpMethod.Post, url, data);

        public HttpRequestMessage CreatePut(string url, object data)
            => Create(HttpMethod.Put, url, data);

        public HttpRequestMessage CreateDelete(string url, object data = null)
            => Create(HttpMethod.Delete, url, data);
    }
}
