using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings
{
    /// <summary>
    /// BG 設定檔案
    /// </summary>
    public record BGConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }

        public string SecretKey { get; init; }

        public string SecretCode { get; init; }
    }
}
