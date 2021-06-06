using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Wrapic.Test.Tests
{
    public class WrapicSettingsTest
    {
        [Fact]
        public void Retry()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    ("WrapicSettings:Retry", "123"))
                .Build();

            var settings = new WrapicSettings(configuration);
            var actual = settings.Retry;
            Assert.Equal(123, actual);
        }
        
        [Fact]
        public void GetUrls()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    ("WrapicSettings:BaseUrls:Example", "https://example.com"))
                .Build();

            var settings = new WrapicSettings(configuration);
            var actual = settings.GetBaseUrl("Example");
            Assert.Equal("https://example.com", actual);
        }

        [Fact]
        public void GetUrlsNull()
        {
            var configuration = new ConfigurationBuilder()
                .Build();

            var settings = new WrapicSettings(configuration);
            var actual = settings.GetBaseUrl("Example");
            Assert.Null(actual);
        }
    }
}