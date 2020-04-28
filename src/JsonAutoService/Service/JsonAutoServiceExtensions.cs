using System;
using Microsoft.Extensions.DependencyInjection;

namespace JsonAutoService.Service
{
    public static class JsonAutoServiceExtensions
    {
        public static IServiceCollection AddJsonAutoService(this IServiceCollection services, Action<JsonAutoServiceOptions> options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            services.Configure<JsonAutoServiceOptions>(options);
            services.AddTransient<IJsonAutoService, global::JsonAutoService.Service.JsonAutoService>();

            return services;
        }
    }
}
