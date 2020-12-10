using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Game : BaseEntity<int>
    {
        public GameStatus Status { get; set; }

        public string ExternalId { get; set; }

        public string Description { get; set; }

        public string EndpointPath { get; set; }

        public string RuleUrl { get; set; }

        public Guid GameProviderId { get; set; }

        public virtual GameProvider GameProvider { get; set; }

        public GameType Type { get; set; }

        public PlatformType PlatformType { get; set; }

        public bool SubGame { get; set; }

        public bool HasTurnoverOnTieBet { get; set; }

        /// <summary>
        /// Currencies that the game supports, recorded by csv format
        /// Empty for all currencies that game provider has.
        /// </summary>
        public string AvailableCurrencies { get; set; }
        
        public virtual ICollection<BrandGame> Brands { get; set; }
    }
}