#if NETSTANDARD2_1
using Microsoft.Extensions.Configuration;
using UGS.Infrastructures.JsonConfig;

namespace UGS.Infrastructures.Redis
{
    public sealed class RedisConfig : IRedisConfig
    {
        private const string Prefix = "Ugs";
        private static IConfiguration Configuration => JsonConfiguration.Build();

        string IRedisConfig.RedisConfigurationString =>
            Configuration[Prefix + "RedisConfigurationString"];

        string IRedisConfig.SignalRRedisConnectionString =>
            Configuration[Prefix + "SignalRRedisConnectionString"];
    }
}
#endif