using System;
using System.Collections.Concurrent;

namespace TGS.Apis.LogApi.Controllers.Messages
{
    public class StatusModel
    {
        public string MachineName { get; set; }
        public string HostName { get; set; }
        public string HostIp { get; set; }
        public string Environment { get; set; }
        public string OsVersion { get; set; }
        public int ProcessCount { get; set; }
        public bool CanConnectLogDb { get; set; }
        public string RedisConnectionStatus { get; set; }
        public string last_error { get; set; }
        public string last_event { get; set; }
        public DateTimeOffset last_received_event_time { get; set; }
        public DateTimeOffset last_log_insert_time { get; set; }
        public DateTimeOffset last_startup_time { get; set; }
        public int batch_insert_count { get; set; }
        public double batch_insert_interval { get; set; }
        public ConcurrentDictionary<string, uint> log_received_count { get; set; }
    }
}
