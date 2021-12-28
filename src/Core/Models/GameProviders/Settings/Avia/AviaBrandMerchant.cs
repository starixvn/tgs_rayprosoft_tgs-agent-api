using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings.Avia
{
    public class AviaBrandMerchant
    {

        public string BrandCode { get; init; }

        public Uri ApiUrl { get; init; }

        public string Authorization { get; set; }

        public string PlayerPassword { get; init; }

        public bool Status { get; init; }
    }
}
