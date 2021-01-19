using System.Collections.Generic;

namespace TGS.Core.Models.Messages.HealthCheck
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
        public string Version { get; init; }
        public int ProcessCount { get; init; }
        public IReadOnlyDictionary<string, object> AdditionalValues { get; init; }
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
        public string Description { get; set; }
        public double Duration { get; init; }
        public string Tags { get; init; }
        public IReadOnlyDictionary<string, object> Data { get; init; }
        public string Exception { get; init; }
    }
}