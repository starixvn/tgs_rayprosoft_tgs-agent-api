using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.WGSports.Outgoing
{
    public record WGSportsPrepareLoginRequest
    {
        public long Timestamp { get; init; }
        public string PlayerCode { get; init; }
        public string PlayerNickname { get; init; }
        public string CurrencyCode { get; init; }
        public string Handicap { get; init; }
    }
}
