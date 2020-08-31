using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public class GetCachedGameProviderResult
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public string Description { get; set; }
        public bool AutoClosedRound { get; set; }
        public bool SupportTestPlayer { get; set; }
        public int PlaceBetTimeout { get; set; }
        public int SettleBetTimeout { get; set; }
    }
}