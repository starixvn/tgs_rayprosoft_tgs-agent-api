using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public record GetCachedGameProviderResult
    {
        public Guid Id { get; init; }
        public string Code { get; init; }
        public string Name { get; init; }
        public Status Status { get; init; }
        public string Description { get; init; }
        public bool AutoCloseRound { get; init; }
        public bool SupportTestPlayer { get; init; }
        public int PlaceBetTimeout { get; init; }
        public int SettleBetTimeout { get; init; }
        public bool AllowKickPlayer { get; init; }
        public bool TransferWallet { get; init; }
    }
}