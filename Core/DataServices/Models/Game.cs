using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Game : BaseEntity
    {
        public GameStatus Status { get; set; }

        public string ExternalId { get; set; }

        public string Description { get; set; }

        public string EndpointPath { get; set; }

        public string RuleUrl { get; set; }

        public Guid GameProviderId { get; set; }

        public GameProvider GameProvider { get; set; }

        public GameType GameType { get; set; }

        public PlatformType PlatformType { get; set; }

        public virtual ICollection<BrandGame> Brands { get; set; }
    }
}