using Microsoft.Extensions.Configuration;

namespace TGS.Core.Infrastructures.Redis
{
    public sealed class RedisConfig : IRedisConfig
    {
        private readonly IConfiguration _configuration;
        public RedisConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        string IRedisConfig.RedisConfigurationString =>
            _configuration.GetConnectionString("RedisConfigurationString");
    }
}