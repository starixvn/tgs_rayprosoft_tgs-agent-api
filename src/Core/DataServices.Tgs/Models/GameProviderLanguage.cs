using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class GameProviderLanguage
    {
        public Guid GameProviderId { get; set; }
        public virtual GameProvider GameProvider { get; set; }
        public string CultureCode { get; set; }
        public virtual Language Language { get; set; }
        public string GameProviderCultureCode { get; set; }
    }
}