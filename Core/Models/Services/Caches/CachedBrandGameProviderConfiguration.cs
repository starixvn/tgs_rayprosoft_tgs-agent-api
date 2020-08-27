using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.Services.Caches
{
    public class CachedBrandGameProviderConfiguration
    {
        public Guid GameProviderId { get; set; }
        public Guid GameProviderConfigurationId { get; set; }
    }
}