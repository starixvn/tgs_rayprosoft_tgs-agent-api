using System;

namespace TGS.Core.DataServices.Models
{
    public class GameProviderLanguage
    {
        public Guid GameProviderId { get; set; }
        public GameProvider GameProvider { get; set; }
        public string CultureCode { get; set; }
        public Language Language { get; set; }
        public string GameProviderCultureCode { get; set; }
    }
}