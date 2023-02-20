namespace Mango.web.Models.Factories
{
    public static class FactoryHelper
    {
        public static void AddFactories(this IServiceCollection services)
        {
            services.AddTransient<HttpRequestMessage>();
            services.AddSingleton<Func<HttpRequestMessage>>(x => () => x.GetService<HttpRequestMessage>());
            services.AddSingleton<IHttpRequestMessageFactory, HttpRequestMessageFactory>();
        }
    }
}
