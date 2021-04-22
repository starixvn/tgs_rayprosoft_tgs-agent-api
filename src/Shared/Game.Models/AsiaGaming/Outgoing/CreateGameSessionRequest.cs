using System;

namespace TGS.Shared.Game.Models.AsiaGaming.Outgoing
{
    public class CreateGameSessionRequest
    {
        public string productid { get; set; }

        public string username { get; set; }

        public Guid session_token { get; set; }

        public decimal credit { get; set; }
    }
}