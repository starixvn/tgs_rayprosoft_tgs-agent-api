using System;
using TGS.Core.Models.GameProviders.Settings.JDB;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record JDBConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }

        public int TimezoneHour { get; init; }

        public JDBAgent[] Agents { get; init; }
    }
}
