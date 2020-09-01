using System;
using TGS.Infrastructures.Log.Enums;

namespace TGS.Apis.LogApi.DataService.Models
{
    public class ServiceLog
    {
        public long Id { get; set; }
        public DateTimeOffset RecordedOn { get; set; }
        public string ServerName { get; set; }
        public TgsLogLevel Level { get; set; }
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }

    }
}
