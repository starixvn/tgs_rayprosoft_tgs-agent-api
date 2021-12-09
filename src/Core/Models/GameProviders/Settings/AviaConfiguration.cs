using System;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record AviaConfiguration : PluggableConfigurationBase
    {

        public decimal AmountMaximum { get; set; }

        public string AmountFormat { get; set; }


        public Uri ApiUrl { get; init; }

        public string Authorization { get; set; }
    }
}
