using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.Shared.Game.Models.SexyGaming.Common
{
    public record SexyGamingGetPlayerInfo
    {
        public Guid GameProviderConfigurationId { get; init; }
        public string PlayerCode { get; init; }
        public PlatformType PlatformType { get; init; }
    }
}
