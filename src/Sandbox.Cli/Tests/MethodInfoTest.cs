using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Sandbox.Cli.Tests
{
    public class MethodInfoTest
    {
        private readonly ILogger<MethodInfoTest> _logger;

        public MethodInfoTest(ILogger<MethodInfoTest> logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            var tcs = typeof(TaskCompletionSource<>).MakeGenericType(typeof(string));
            // ts
        }
    }
}