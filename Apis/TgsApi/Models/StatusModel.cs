namespace TGS.Apis.TgsApi.Models
{
    public record StatusModel
    {
        public string MachineName { get; init; }
        public string HostName { get; init; }
        public string HostIp { get; init; }
        public string Environment { get; init; }
        public string OsVersion { get; init; }
        public int ProcessCount { get; init; }
        public bool CanConnectTgsDb { get; init; }
        public bool CanConnectBetDb { get; init; }
        public string RedisConnectionStatus { get; init; }
        public string GameProviderCode { get; init; }
        public bool FeatureEnabled { get; init; }
    }
}
