using System;

namespace TGS.Apis.LogApi.DataService.Models
{
    public class EventLog
    {
        public Guid Id { get; set; }
        public string Module { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public DateTimeOffset RecordedOn { get; set; }

    }
}
