using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wrapic
{
    public class WrapicOptions
    {
        private readonly IServiceCollection _services;
        public WrapicOptions(IServiceCollection services)
        {
            _services = services;
        }

        public IWrapicSerializer Serializer { get; set; } = new WrapicJsonSerializer();

        public WrapicOptions AddApi<T>(string baseUrlName, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            _services.Add(new ServiceDescriptor(typeof(T), provider =>
            {
                var settings = provider.GetService<IWrapicSettings>();
                var baseUrl = settings?.GetBaseUrl(baseUrlName);
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception($"{nameof(baseUrlName)}: '{baseUrlName}' が設定されていません");
                }

                var httpClient = provider.GetService<HttpClient>();
                if (httpClient == null)
                {
                    throw new Exception($"{nameof(httpClient)} が設定されていません");
                }

                return WrapicClient.Create<T>(httpClient, baseUrl, Serializer);
            }, lifetime));

            return this;
        }
    }
}