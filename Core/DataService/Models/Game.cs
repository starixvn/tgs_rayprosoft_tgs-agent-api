using System;
using TGS.Core.DataServices.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Game : BaseEntity
    {
        public string ExternalId { get; set; }

        public string Description { get; set; }

        public string EndpointPath { get; set; }

        public string DemoUrl { get; set; }

        public string RuleUrl { get; set; }

        public int DisplayOrder { get; set; }

        public Guid GameProviderId { get; set; }

        public GameProvider GameProvider { get; set; }

        public GameType GameType { get; set; }

        public PlatformType PlatformType { get; set; }

        public bool ZeroTurnoverOnTieBet { get; set; }



    }
}