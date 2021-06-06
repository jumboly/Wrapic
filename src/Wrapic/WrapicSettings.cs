using Microsoft.Extensions.Configuration;

namespace Wrapic
{
    public interface IWrapicSettings
    {
        int Retry { get; }
        string GetBaseUrl(string baseUrlName);
    }

    public class WrapicSettings : IWrapicSettings
    {
        private IConfigurationSection _baseUrls;
        
        public WrapicSettings(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(WrapicSettings));
            
            if (!int.TryParse(section[nameof(Retry)], out var retry))
            {
                retry = 2; // デフォルト値
            }
            Retry = retry;
            
            _baseUrls = section.GetSection("BaseUrls");
        }
        
        public int Retry { get; set; }
        
        public string GetBaseUrl(string baseUrlName)
        {
            return _baseUrls[baseUrlName];
        }
    }
}