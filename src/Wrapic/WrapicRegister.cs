using System;
using Microsoft.Extensions.DependencyInjection;

namespace Wrapic
{
    public static class WrapicRegister
    {
        public static IServiceCollection AddWrapic(this IServiceCollection services, Action<WrapicOptions> optionHandler)
        {
            var options = new WrapicOptions(services);
            optionHandler.Invoke(options);

            services.AddSingleton<IWrapicSettings, WrapicSettings>();

            return services;
        }
    }
}