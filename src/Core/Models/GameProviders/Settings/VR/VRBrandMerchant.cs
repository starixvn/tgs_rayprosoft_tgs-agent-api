using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings.VR
{
    public record VRBrandMerchant
    {
        public string BrandCode { get; init; }

        public Uri ApiUrl { get; init; }

        public string Version { get; init; }

        public string Merchantid { get; init; }

        public string Deskey { get; init; }

        public bool Status { get; set; }
    }
}
