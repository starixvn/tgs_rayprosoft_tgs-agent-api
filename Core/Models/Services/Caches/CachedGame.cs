using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedGame
    {
        public Guid Id;
        public string Code;
        public string Name;
        public GameStatus Status;
        public string ExternalId;
        public bool SubGame;
        public string RuleUrl;
        public string Endpoint;
        public GameType Type;
        public PlatformType PlatformType;
        public bool HasTurnoverOnTieBet;
    }
}
