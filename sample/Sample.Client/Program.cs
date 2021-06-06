using System;
using System.Net.Http;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Api;
using Sample.Api.Dto;
using Wrapic;

namespace Sample.Client
{
    class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<HttpClient>();
                    services.AddWrapic(options =>
                    {
                        options.AddApi<IFooApi>("Foo");
                    });
                })
                .RunConsoleAppFrameworkAsync<Program>(args);
        }

        private readonly ILogger<Program> _logger;
        private readonly IFooApi _foo;

        public Program(ILogger<Program> logger, IFooApi foo)
        {
            _logger = logger;
            _foo = foo;
        }

        public async Task Run()
        {
            await _foo.Put(new Foo {Id = 1, Name = "Aさん"});
            await _foo.Put(new Foo {Id = 2, Name = "Bさん"});
            await _foo.Put(new Foo {Id = 3, Name = "Cさん"});

            var a = await _foo.GetById(1);
            _logger.LogInformation("{0}", a);

            await _foo.DeleteById(2);
            
            foreach (var foo in await _foo.GetAll())
            {
                _logger.LogInformation("{0}", foo);
            }
        }
    }
}