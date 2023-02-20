using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MangoLibrary
{
    public static class MappingConfig
    {
        public static IServiceCollection AddMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> action)
        {
            IMapper mapper = new MapperConfiguration(action).CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
