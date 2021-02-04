using System;
using MessagePack;

// ReSharper disable InconsistentNaming

namespace TGS.Core.Infrastructures.Log.Events
{
    [MessagePackObject((true))]
    public class ApiLogEvent : LogBusEvent
    {
        public Guid trackingid { get; set; }
        public string ip { get; set; }
        public string path { get; set; }
        public string method { get; set; }
        public string prefix { get; set; }
        public string servername { get; set; }
        public string requestheaders { get; set; }
        public string requestcontent { get; set; }
        public string responseheaders { get; set; }
        public string responsecontent { get; set; }
        public int responsecode { get; set; }
        public int executiontimems { get; set; }
        public DateTimeOffset recordedon { get; set; }
    }
}
