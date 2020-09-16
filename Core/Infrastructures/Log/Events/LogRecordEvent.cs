using System;
using System.Runtime.Serialization;
using MessagePack;
using TGS.Core.Infrastructures.Log.Enums;

// ReSharper disable InconsistentNaming

namespace TGS.Core.Infrastructures.Log.Events
{
    [MessagePackObject(true)]
    public class LogRecordEvent : LogBusEvent
    {
        public Guid trackingid { get; set; }
        public DateTimeOffset recordedon { get; set; }
        public string servername { get; set; }
        public TgsLogLevel level { get; set; }
        public string logger { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string exception { get; set; }


        [DataMember(EmitDefaultValue = false)]
        public string file { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string member { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int line { get; set; }
    }
}
