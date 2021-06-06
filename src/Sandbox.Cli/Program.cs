using System;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;

namespace Sandbox.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #if DEBUG
            args = new[]
            {
                "MethodInfoTest.Run"
            };
            #endif
            
            await Host.CreateDefaultBuilder()
                .RunConsoleAppFrameworkAsync(args);
        }
    }
}