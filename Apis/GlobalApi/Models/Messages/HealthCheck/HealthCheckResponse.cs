namespace TGS.Apis.GlobalApi.Models.Messages.HealthCheck
{
    public record HealthCheckResponse
    {
        public Data Data { get; init; }
        public Result Result { get; init; }
    }

    public record Data
    {
        public string MachineName { get; init; }
        public string HostName { get; init; }
        public string HostIp { get; init; }
        public string Environment { get; init; }
        public string OsVersion { get; init; }
        public int ProcessCount { get; init; }
    }

    public record Result
    {
        public string Status { get; init; }
        public double TotalDuration { get; init; }
        public DependencyService[] DependencyServices { get; init; }
    }

    public record DependencyService
    {
        public string Key { get; init; }
        public string Status { get; init; }
        public double Duration { get; init; }
        public string Tags { get; init; }
        public string Exception { get; init; }
    }
}