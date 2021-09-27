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
        public byte[] WithdrawPassword { get; init; } = Array.Empty<byte>();
        public byte[] WithdrawSalt { get; init; } = Array.Empty<byte>();
        public string CurrencyCode { get; init; }
        public string ExternalCurrencyCode { get; init; }
        public int BetLimitId { get; init; }
        public PlayerType Type { get; init; }
        public Guid? AgentId { get; init; }
        public string AgentInvitationCode { get; init; }
        public PlayerStatus PlayerStatus { get; init; }
    }
}