using System;

namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    public record CreateGameSessionRequest
    {
        public string productid { get; init; }

        public string username { get; init; }

        public Guid session_token { get; init; }

        public decimal credit { get; init; }
    }
}