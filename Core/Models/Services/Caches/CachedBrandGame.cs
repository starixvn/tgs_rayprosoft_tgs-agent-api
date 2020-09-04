using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Caches
{
    public record CachedBrandGame
    {
        public Guid GameProviderId { get; set; }

        public Guid GameId { get; set; }
        
        public string ExternalGameId { get; set; }
        
        public PlatformType PlatformType { get; set; }
        
        public bool SubGame { get;set;}
    }
}
