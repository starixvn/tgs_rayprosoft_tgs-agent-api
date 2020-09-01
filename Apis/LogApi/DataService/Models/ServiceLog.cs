using System;

namespace TGS.Apis.LogApi.DataService.Models
{
    public class ServiceLog
    {
        public long Id { get; set; }
        public DateTimeOffset RecordedOn { get; set; }
        public string MachineName { get; set; }
        public string Level { get; set; }
        public string ServerAddress { get; set; }
        public string RemoteAddress { get; set; }
        public string Logger { get; set; }
        public string Port { get; set; }
        public string Properties { get; set; }
        public string Callsite { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }

    }
}
