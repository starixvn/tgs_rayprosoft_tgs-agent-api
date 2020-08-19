namespace TGS.Infrastructures.Redis
{
    public sealed class RedisConfig : IRedisConfig
    {
        private const string Prefix = "Ugs";

        string IRedisConfig.RedisConfigurationString =>
            ConfigurationManager.AppSettings[Prefix + "RedisConfigurationString"];

        string IRedisConfig.SignalRRedisConnectionString =>
           ConfigurationManager.AppSettings[Prefix + "SignalRRedisConnectionString"];
    }
}