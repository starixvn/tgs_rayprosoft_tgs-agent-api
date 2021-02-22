using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public record RoundWithGameActionExternalIds
    {
        public Guid Id { get; init; }
        public string ExternalRoundId { get; init; }
        public string PlayerCode { get; init; }
        public string CurrencyCode { get; init; }
        public PlatformType PlatformType { get; init; }
        public int GameId { get; init; }
        public string ExternalGameId { get; init; }
        public RoundStatus Status { get; init; }
        public DateTimeOffset ExternalCreatedOn { get; init; }
        public GameActionExternalIds[] GameActionExternalIds { get; init; }
    }
}