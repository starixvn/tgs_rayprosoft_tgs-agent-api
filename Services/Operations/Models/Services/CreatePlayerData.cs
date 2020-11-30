using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Models.Services
{
    public record CreatePlayerData
    {
        public Guid BrandId { get; init; }
        public string Username { get; init; }
        public byte[] Salt { get; init; }
        public byte[] Password { get; init; }
        public string CultureCode { get; init; }
        public string CurrencyCode { get; init; }
        public string ExternalCurrencyCode { get; init; }
        public string ParentPlayerCode { get; init; }
        public string ParentInvitationCode { get; init; }
        public int BetLimitGroup { get; init; }
        public PlayerType Type { get; init; }
        public bool HasAgent { get; init; }
    }
}