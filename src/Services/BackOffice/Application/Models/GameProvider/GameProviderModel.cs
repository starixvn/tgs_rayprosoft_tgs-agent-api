using System;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.GameProviderBetLimit;

namespace TGS.Services.BackOffice.Application.Models.GameProvider
{
    public class GameProviderModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public string Description { get; set; }
        public bool AutoCloseRound { get; set; }
        public bool SupportTestPlayer { get; set; }
        public bool AllowKickPlayer { get; set; }
        public int PlaceBetTimeout { get; set; }
        public int SettleBetTimeout { get; set; }
        public ushort UniqueId { get; set; }
        public GameProviderCurrencyModel[] Currencies { get; set; }
        public GameProviderLanguageModel[] Languages { get; set; }
        public GameProviderBetLimitModel[] BetLimits { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string UpdatedByUsername { get; set; }
    }
}