using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingGPGameRecordRequest
    {
        public string CurrencyCode { get; set; }
        public DateTimeOffset StartTime { get; init; }
        public DateTimeOffset EndTime { get; init; }
        public int Page { get; set; }
        public int Rows { get; set; }
    }
}
