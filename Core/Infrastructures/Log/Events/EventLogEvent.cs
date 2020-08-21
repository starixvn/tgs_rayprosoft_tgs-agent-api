using System;
using System.Runtime.Serialization;
using MessagePack;
using TGS.Infrastructures.Log.Enums;

// ReSharper disable InconsistentNaming

namespace TGS.Infrastructures.Log.Events
{
    [MessagePackObject((true))]
    public class EventLogEvent : LogBusEvent
    {
        public Guid trackingid { get; set; }
        public DateTimeOffset recordedon { get; set; }

        public TgsLogLevel LogLevel { get; set; }
        public string servername { get; set; }
        public string servicename { get; set; }
        public string message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string exception { get; set; }
    }
}
