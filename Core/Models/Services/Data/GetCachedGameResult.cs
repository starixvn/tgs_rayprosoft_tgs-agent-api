using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public record GetCachedGameResult
    {
        public int Id { get; init; }
        public string Code { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public GameStatus Status { get; init; }
        public string ExternalId { get; init; }
        public bool SubGame { get; init; }
        public string RuleUrl { get; init; }
        public string EndpointPath { get; init; }
        public GameType Type { get; init; }
        public PlatformType PlatformType { get; init; }
        public bool HasTurnoverOnTieBet { get; init; }
        public string AvailableCurrencies { get; init; }
    }
}