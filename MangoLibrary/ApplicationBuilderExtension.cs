using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MangoLibrary
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            var serviceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLifeTime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLifeTime.ApplicationStarted.Register(async () => await serviceBusConsumer.Start());
            hostApplicationLifeTime.ApplicationStopped.Register(async () => await serviceBusConsumer.Stop());

            return app;
        }
    }
}
